using Project4.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Project4.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }

        public virtual DbSet<AboutUs> AboutUss { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Chapter> Chapters { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CustomUser> CustomUsers { get; set; }

        public virtual DbSet<Pagee> Pagees { get; set; }

        public virtual DbSet<Story> Stories { get; set; }

        public virtual DbSet<Story_Category> Story_Categories { get; set; }

        public virtual DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }
        public virtual DbSet<Viewed> Vieweds { get; set; }

    }
}
