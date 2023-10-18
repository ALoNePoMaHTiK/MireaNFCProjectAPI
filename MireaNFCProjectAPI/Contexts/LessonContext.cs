using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Contexts
{
    public class LessonContext : DbContext
    {
        public LessonContext(DbContextOptions<LessonContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Lesson> Lessons { get; set; }
    }
}