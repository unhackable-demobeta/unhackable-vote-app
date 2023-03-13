using Microsoft.EntityFrameworkCore;
using VoteApp.PostgreSQL;
using VoteApp;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteApp.PostgreSQL
{
    public class VotesContext : DbContext
    {
        public VotesContext(){
        }

        public DbSet<Vote> Votes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location)).AddJsonFile("appsettings.json");
            var configuration = configBuilder.Build();
            var connectionString = configuration.GetValue<string>("DatabaseConnectionConfig");
            optionsBuilder.UseNpgsql(connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasNoKey();
                entity.HasKey("id");
                entity.ToTable("votes");
            });
        }
    }

    public class Vote
    {
        [Column("id")]
        public int id { get; set; }
        public string vote { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
