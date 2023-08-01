using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
    }
}