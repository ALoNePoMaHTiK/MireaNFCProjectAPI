using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutsConteoller : ControllerBase
    {
        IDbContextFactory<CheckoutContext> _contextFactory;

        public CheckoutsConteoller(IDbContextFactory<CheckoutContext> contextFactory) => _contextFactory = contextFactory;

        [HttpGet]
        public async Task<IEnumerable<Checkout>> GetAll()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Checkouts.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<Checkout> Get(Guid id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Checkouts.FindAsync(id);
        }
        [HttpGet("ByTag/{tagId}")]
        public async Task<IEnumerable<Checkout>> GetByTag(string tagId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Checkouts.Where(c => c.TagId == tagId).ToListAsync();
        }

        [HttpPost]
        public async Task<Group> Create([FromBody] Group group)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            context.Add(group);
            await context.SaveChangesAsync();
            return group;
        }
    }
}
