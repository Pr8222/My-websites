using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<RoleKeys> RoleKeys { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<UserExtraKeys> UserExtraKeys { get; set; }
    }
}
