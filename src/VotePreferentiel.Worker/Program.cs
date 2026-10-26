using VotePreferentiel.Worker;
using VotePreferentiel;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient<VoteProvider>();
builder.Services.AddSingleton<IVoteCounter, VoteCounter>();
builder.Services.AddSingleton(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var url = builder.Configuration["VotesUrl"] ??
              Environment.GetEnvironmentVariable("VOTE_PREFERENTIEL_URL");
    return new VoteProvider(httpClientFactory.CreateClient(), url);
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
