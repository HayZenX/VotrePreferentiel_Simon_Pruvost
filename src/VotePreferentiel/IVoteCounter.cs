namespace VotePreferentiel;

public interface IVoteCounter
{
    HashSet<string> GetCandidates(VoteModel voteModel);

    string GetWinner(VoteModel voteModel);

    List<string> GetAllFirstChoices(VoteModel voteModel);

    IDictionary<string, int> CountFirstChoices(List<string> firstChoices);

    string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts);
}
