using AlkemyChallenge.Models;
using Microsoft.EntityFrameworkCore;
namespace AlkemyChallenge.Data

{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Movie> Movies { get; set; }    
        public DbSet<Genre> Genres { get; set; }


    }
}
