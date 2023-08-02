using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Contexts
{
    public class RoomContext : DbContext
    {
        public RoomContext(DbContextOptions<RoomContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Room> Rooms { get; set; }
    }
}