using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;
using System.Text.RegularExpressions;

namespace MireaNFCProjectAPI.Controllers
{
    //[ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        IDbContextFactory<LessonContext> _contextFactory;

        public LessonsController(IDbContextFactory<LessonContext> contextFactory) => _contextFactory = contextFactory;

        [HttpGet]
        public async Task<IEnumerable<Lesson>> GetAll()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Lessons.ToListAsync();
        }

        /// <summary>
        /// Получение пары по идентификатору
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Lesson> Get(string id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Lessons.FindAsync(id);
        }

        /// <summary>
        ///     Получение пары по времени
        /// </summary>
        [HttpGet("ByDateTime/{groupId}/{datetime:datetime}")]
        public async Task<Lesson> GetByDateTime(string groupId,DateTime datetime)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Lessons.Where(l => l.StartDateTime < datetime && l.FinishDateTime > datetime && l.GroupId == groupId).FirstOrDefaultAsync();
        }
    }
}
