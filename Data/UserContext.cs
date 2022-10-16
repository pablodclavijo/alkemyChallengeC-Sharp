using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AlkemyChallenge.Models;

namespace AlkemyChallenge.Data
{
    public class UserContext : IdentityDbContext<User>
    {
        private const string schema = "users";
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(schema);
        }
       // public DbSet<User> Users { get; set; }

    }



}
