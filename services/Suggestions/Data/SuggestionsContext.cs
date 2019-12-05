using Microsoft.EntityFrameworkCore;
using Suggestions.Models;

namespace Suggestions.Data
{
    public class SuggestionsContext : DbContext
    {
        public DbSet<Suggestion> Suggestions { get; set; }

        public SuggestionsContext(DbContextOptions<SuggestionsContext> options) : base(options) { }
    }
}