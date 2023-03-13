namespace vote_worker;
using VoteApp.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Redis;
using System.Text.Json;
using System;
using System.Reflection;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public class JVote
    {
        public string voter_id { get; set; }
        public string vote { get; set; }
        public List<Vote> Votes { get; set; }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string jsonVote = FetchVote();
            if(!String.IsNullOrEmpty(jsonVote))
            {
                try
                {
                    var jsonString = JsonSerializer.Deserialize<JVote>(jsonVote);
                    var vote = jsonString.vote;
                    RecordVote(vote);
                }
                catch (Exception e)
                {
                    _logger.LogError("Vote json string fetched from redis malfored: {error}", e);
                }
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    protected string FetchVote()
    {
        string vote = "";
        var configBuilder = new ConfigurationBuilder().SetBasePath( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location)).AddJsonFile("appsettings.json");
        var configuration = configBuilder.Build();
        var connectionString = configuration.GetValue<string>("RedisConnectionConfig");

        var manager = new RedisManagerPool(connectionString);
        try
        {
            using (var client = manager.GetClient())
            {
                vote = client.RemoveStartFromList("votes");
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to connect to redis and fetch data: {error}", e);
        }

        return vote;
    }

    protected void RecordVote(string vote)
    {
            using (var db = new VotesContext())
            {
                var avote = new Vote();
                avote.vote = vote;
                try
                {
                        db.Votes.Add(avote);
                        db.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError("Failed to write vote to Postgres: {error}", e);
                }
            }
    }
}
