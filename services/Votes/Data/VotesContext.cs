using Microsoft.EntityFrameworkCore;
using Votes.Models;

namespace Votes.Data
{
    public class VotesContext : DbContext
    {
        public DbSet<Vote> Votes { get; set; }

        public VotesContext(DbContextOptions<VotesContext> options) : base(options) { }
    }
}