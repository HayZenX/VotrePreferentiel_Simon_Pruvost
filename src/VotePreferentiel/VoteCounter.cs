namespace VotePreferentiel;

public sealed class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        return voteModel.Votes
            .SelectMany(vote => vote)
            .Where(candidate => !string.IsNullOrWhiteSpace(candidate))
            .ToHashSet(StringComparer.Ordinal);
    }

    public string GetWinner(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        var activeCandidates = GetCandidates(voteModel);
        if (activeCandidates.Count == 0)
        {
            throw new InvalidOperationException("Impossible de déterminer un gagnant sans bulletin valide.");
        }

        while (activeCandidates.Count > 1)
        {
            var currentChoices = voteModel.Votes
                .Select(vote => vote.FirstOrDefault(activeCandidates.Contains))
                .Where(choice => choice is not null)
                .Select(choice => choice!)
                .ToList();

            if (currentChoices.Count == 0)
            {
                throw new InvalidOperationException("Aucun bulletin ne permet de départager les candidats.");
            }

            var counts = CountFirstChoices(currentChoices);
            var candidatesToEliminate = GetCandidatesWithLeastVotes(counts);

            if (candidatesToEliminate.Length >= activeCandidates.Count)
            {
                throw new InvalidOperationException("Égalité complète impossible à départager.");
            }

            foreach (var candidate in candidatesToEliminate)
            {
                activeCandidates.Remove(candidate);
            }
        }

        return activeCandidates.Single();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        ArgumentNullException.ThrowIfNull(voteModel);

        return voteModel.Votes
            .Where(vote => vote.Count > 0)
            .Select(vote => vote[0])
            .Where(choice => !string.IsNullOrWhiteSpace(choice))
            .ToList();
    }

    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        ArgumentNullException.ThrowIfNull(firstChoices);

        return firstChoices
            .Where(choice => !string.IsNullOrWhiteSpace(choice))
            .GroupBy(choice => choice, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        ArgumentNullException.ThrowIfNull(firstChoiceCounts);
        if (firstChoiceCounts.Count == 0)
        {
            throw new InvalidOperationException("Aucun candidat n'a obtenu de voix.");
        }

        var leastVotes = firstChoiceCounts.Values.Min();
        return firstChoiceCounts
            .Where(pair => pair.Value == leastVotes)
            .Select(pair => pair.Key)
            .ToArray();
    }
}
