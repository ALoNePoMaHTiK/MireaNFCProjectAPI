using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Contexts
{
    public class TagContext : DbContext
    {
        public TagContext(DbContextOptions<TagContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Tag> Tags { get; set; }
    }
}