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

        public TagsController(IDbContextFactory<TagContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<Tag>> Get()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tags.ToListAsync();
        }
    }
}
