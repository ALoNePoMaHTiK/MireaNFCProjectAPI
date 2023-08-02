using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Contexts
{
    public class CheckoutContext : DbContext
    {
        public CheckoutContext(DbContextOptions<CheckoutContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Checkout> Checkouts { get; set; }
    }
}