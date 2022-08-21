using Microsoft.EntityFrameworkCore;
using MyBlog.Domain.Core;

namespace MyBlog.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            if (Database.EnsureCreated())
            {
                Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "User" }
                    );
                SaveChanges();

                var generate = new GenerateData(this);

                generate.CreateData();
            }
        }
    }
}
