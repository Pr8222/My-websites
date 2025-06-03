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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleUser>()
                .HasKey(ru => new { ru.RoleId, ru.UserId });

            modelBuilder.Entity<RoleUser>()
                .HasOne(ru => ru.Role)
                .WithMany(r => r.RoleUsers)
                .HasForeignKey(ru => ru.RoleId);

            modelBuilder.Entity<RoleUser>()
                .HasOne(ru => ru.User)
                .WithMany(u => u.RoleUsers)
                .HasForeignKey(ru => ru.UserId);

            modelBuilder.Entity<RoleKeys>()
                .HasKey(rk => new { rk.RoleId, rk.KeyId });

            modelBuilder.Entity<RoleKeys>()
                .HasOne(rk => rk.Role)
                .WithMany(r => r.RoleKeys)
                .HasForeignKey(rk => rk.RoleId);

            modelBuilder.Entity<RoleKeys>()
                .HasOne(rk => rk.Key)
                .WithMany(k => k.RoleKeys)
                .HasForeignKey(rk => rk.KeyId);

            modelBuilder.Entity<UserExtraKeys>()
                .HasOne(uek => uek.User)
                .WithMany(u => u.UserExtraKeys)
                .HasForeignKey(uek => uek.UserId);

            modelBuilder.Entity<UserExtraKeys>()
                .HasOne(uek => uek.Key)
                .WithMany(k => k.UserKeys)
                .HasForeignKey(uek => uek.KeyId);
        }

    }
}
