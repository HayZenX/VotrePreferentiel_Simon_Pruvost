using System.Net.Http.Json;

namespace VotePreferentiel;

public sealed class VoteProvider(HttpClient httpClient, string? votesUrl = null)
{
    public async Task<VoteModel> GetVotesAsync(CancellationToken cancellationToken = default)
    {
        var url = votesUrl ?? Environment.GetEnvironmentVariable("VOTE_PREFERENTIEL_URL");
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException(
                "L'URL du fichier de votes est absente. Configurez VOTE_PREFERENTIEL_URL.");
        }

        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var votes = await response.Content.ReadFromJsonAsync<VoteModel>(cancellationToken);
        return votes ?? throw new InvalidOperationException(
            "Le fichier de votes est vide ou ne contient pas un JSON valide.");
    }
}
