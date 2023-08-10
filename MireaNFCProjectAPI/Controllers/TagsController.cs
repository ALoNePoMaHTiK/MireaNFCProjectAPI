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

        /// <summary>
        /// Получение NFC-метки по серийному номеру
        /// </summary>
        [HttpGet("{tagId}")]
        public async Task<IActionResult> Get(string tagId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var tag = await context.Tags.FindAsync(tagId);
            if (tag == null)
            {
                return new NotFoundResult();
            }
            return Ok(tag);
        }
        [HttpGet("{tagId}/{note}")]
        public async Task<IActionResult> Get(string tagId, string note)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var tag = await context.Tags.Where(t => t.TagId == tagId && t.Note == note).FirstAsync();
            if (tag == null)
            {
                return new NotFoundResult();
            }
            return Ok(tag);
        }

        /// <summary>
        /// Получение списка NFC-меток по идентификатору аудитории
        /// </summary>
        [HttpGet("ByRoomId/{roomId}")]
        public async Task<IEnumerable<Tag>> GetByRoomId(short roomId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tags.Where(t => t.RoomId == roomId).ToListAsync();
        }


    }
}
