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
    public class UsersController : ControllerBase
    {
        IDbContextFactory<UserContext> _contextFactory;

        public UsersController(IDbContextFactory<UserContext> contextFactory) => _contextFactory = contextFactory;

        [HttpGet]
        public async Task<IEnumerable<User>> GetAll()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); 
            }
            return Ok(user);
        }
    }
}
