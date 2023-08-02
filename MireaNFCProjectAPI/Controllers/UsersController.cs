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
        public async Task<User> Get(string id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.FindAsync(id);
        }
    }
}
