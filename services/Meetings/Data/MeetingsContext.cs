using Meetings.Models;
using Microsoft.EntityFrameworkCore;

namespace Meetings.Data
{
    public class MeetingsContext : DbContext
    {
        public DbSet<Meeting> Meetings { get; set; }

        public MeetingsContext(DbContextOptions<MeetingsContext> options) : base(options) { }
    }
}