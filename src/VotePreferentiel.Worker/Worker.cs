using VotePreferentiel;

namespace VotePreferentiel.Worker;

public sealed class Worker(
    VoteProvider voteProvider,
    IVoteCounter voteCounter,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var voteModel = await voteProvider.GetVotesAsync(stoppingToken);
            var winner = voteCounter.GetWinner(voteModel);
            logger.LogInformation("La gagnante ou le gagnant de l'élection est : {Winner}", winner);
            Console.WriteLine(winner);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Le traitement des votes a été annulé.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Le traitement des votes a échoué.");
            throw;
        }
    }
}
