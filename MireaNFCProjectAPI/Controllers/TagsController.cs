using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        IDbContextFactory<TagContext> _contextFactory;

        public TagsController(IDbContextFactory<TagContext> contextFactory) =>_contextFactory = contextFactory;

        [HttpGet]
        public async Task<IEnumerable<Tag>> GetAll()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tags.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Tag> Get(string id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tags.FindAsync(id);
        }

        [HttpGet("ByRoomId/{roomId}")]
        public async Task<IEnumerable<Tag>> GetByRoomId(short roomId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tags.Where(t => t.RoomId == roomId).ToListAsync();
        }
    }
}
