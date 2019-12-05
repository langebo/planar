using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.Data
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }
    }
}