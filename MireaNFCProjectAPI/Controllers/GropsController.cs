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
    public class GropsController : ControllerBase
    {
        IDbContextFactory<GroupContext> _contextFactory;

        public GropsController(IDbContextFactory<GroupContext> contextFactory) => _contextFactory = contextFactory;

        [HttpGet]
        public async Task<IEnumerable<Group>> GetAll()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Groups.ToListAsync();
        }

        /// <summary>
        /// Получение группы по идентификатору (интернациональное наименование)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Group> Get(string id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Groups.FindAsync(id);
        }

        /// <summary>
        /// Получение группы по названию (русское именование)
        /// </summary>
        [HttpGet("ByName/{title}")]
        public async Task<Group> GetByName(string title)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Groups.Where(n => n.Title == title).FirstAsync();
        }

        /// <summary>
        /// Получение списка групп по идентификатору института
        /// </summary>
        [HttpGet("ByInstitute/{id}")]
        public async Task<Group> GetByInstitute(byte instituteId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Groups.Where(n => n.InstituteId == instituteId).FirstAsync();
        }

        /// <summary>
        /// Добавление новой группы
        /// </summary>
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
