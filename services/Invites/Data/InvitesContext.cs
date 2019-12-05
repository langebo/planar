using Invites.Models;
using Microsoft.EntityFrameworkCore;

namespace Invites.Data
{
    public class InvitesContext : DbContext
    {
        public DbSet<Invite> Invites { get; set; }

        public InvitesContext(DbContextOptions<InvitesContext> options) : base(options) { }
    }
}