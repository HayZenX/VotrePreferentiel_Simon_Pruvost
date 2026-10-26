using VotePreferentiel;

namespace VotePreferentiel.Tests;

public class VoteCounterTests
{
    private readonly IVoteCounter counter = new VoteCounter();

    [Test]
    public async Task GetWinner_ReturnsChloeForTheSubjectExample()
    {
        var votes = new VoteModel(
        [
            ["Alice", "Bilal", "Chloé"],
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Bilal", "Alice"],
            ["Chloé", "Alice", "Bilal"]
        ]);

        await Assert.That(counter.GetWinner(votes)).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_ReturnsTheOnlyCandidate()
    {
        var votes = new VoteModel([["Alice"]]);

        await Assert.That(counter.GetWinner(votes)).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetWinner_TransfersPreferencesAfterEliminations()
    {
        var votes = new VoteModel(
        [
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Alice", "Bilal", "Chloé", "Diane"],
            ["Bilal", "Chloé", "Alice", "Diane"],
            ["Bilal", "Chloé", "Alice", "Diane"],
            ["Bilal", "Chloé", "Alice", "Diane"],
            ["Chloé", "Alice", "Bilal", "Diane"],
            ["Chloé", "Alice", "Bilal", "Diane"],
            ["Chloé", "Alice", "Bilal", "Diane"],
            ["Diane", "Chloé", "Bilal", "Alice"]
        ]);

        await Assert.That(counter.GetWinner(votes)).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_EliminatesTiedLastCandidatesSimultaneously()
    {
        var votes = new VoteModel(
        [
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Alice", "Bilal"],
            ["Chloé", "Bilal", "Alice"]
        ]);

        await Assert.That(counter.GetWinner(votes)).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_ThrowsForAnEmptyBallot()
    {
        var votes = new VoteModel([[]]);

        await Assert.That(() => counter.GetWinner(votes))
            .Throws<InvalidOperationException>();
    }
}
