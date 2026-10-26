using System.Text.Json;
using System.Text.Json.Serialization;

namespace VotePreferentiel;

[JsonConverter(typeof(VoteModelJsonConverter))]
public sealed class VoteModel
{
    public VoteModel()
    {
    }

    public VoteModel(IEnumerable<IEnumerable<string>> votes)
    {
        Votes = votes.Select(vote => vote.ToList()).ToList();
    }

    public List<List<string>> Votes { get; set; } = [];
}

public sealed class VoteModelJsonConverter : JsonConverter<VoteModel>
{
    public override VoteModel Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return new VoteModel(ReadVotes(root));
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (property.Name.Equals("votes", StringComparison.OrdinalIgnoreCase) &&
                    property.Value.ValueKind == JsonValueKind.Array)
                {
                    return new VoteModel(ReadVotes(property.Value));
                }
            }
        }

        throw new JsonException("Le JSON doit contenir un tableau de bulletins.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        VoteModel value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Votes, options);
    }

    private static IEnumerable<IEnumerable<string>> ReadVotes(JsonElement votesElement)
    {
        foreach (var voteElement in votesElement.EnumerateArray())
        {
            var choicesElement = voteElement;
            if (voteElement.ValueKind == JsonValueKind.Object)
            {
                var choicesProperty = voteElement.EnumerateObject()
                    .FirstOrDefault(property =>
                        property.Name.Equals("choices", StringComparison.OrdinalIgnoreCase));

                if (choicesProperty.Value.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException("Chaque bulletin doit contenir un tableau Choices.");
                }

                choicesElement = choicesProperty.Value;
            }

            if (choicesElement.ValueKind != JsonValueKind.Array)
            {
                throw new JsonException("Chaque bulletin doit être un tableau de candidats.");
            }

            var choices = choicesElement.EnumerateArray()
                .Select(choice => choice.GetString())
                .Where(choice => !string.IsNullOrWhiteSpace(choice))
                .Select(choice => choice!)
                .ToList();

            yield return choices;
        }
    }
}
