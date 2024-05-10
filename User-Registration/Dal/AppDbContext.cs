using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using User_Registration.Models;

namespace User_Registration.Dal
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User_Registration.Models.CreateUser>? CreateUser { get; set; }
        public DbSet<User_Registration.Models.UserProfile>? UserProfile { get; set; }
    }
}
