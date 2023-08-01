using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Contexts
{
    public class GroupContext : DbContext
    {
        public GroupContext(DbContextOptions<GroupContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Group> Groups { get; set; }
    }
}