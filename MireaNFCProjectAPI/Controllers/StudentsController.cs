using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;

namespace MireaNFCProjectAPI.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController
    {
        IDbContextFactory<StudentContext> _contextFactory;

        public StudentsController(IDbContextFactory<StudentContext> contextFactory) => _contextFactory = contextFactory;

        [HttpGet]
        public async Task<IEnumerable<Student>> GetAll()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Students.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Student> Get(string id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Students.FindAsync(id);
        }

        [HttpGet("ByGroup/{groupId}")]
        public async Task<IEnumerable<Student>> GetByGroup(string groupId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Students.Where(n => n.GroupId == groupId).ToListAsync();
        }

        [HttpPost]
        public async Task<Student> Create([FromBody] Student student)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            context.Add(student);
            await context.SaveChangesAsync();
            return student;
        }

        [HttpPut]
        public async Task Update([FromBody] Student student)
        {
            var context = _contextFactory.CreateDbContext();
            Student studentToUpdate = await context.Students.FindAsync(student.StudentId);
            if (studentToUpdate != null)
            {
                studentToUpdate.Email = studentToUpdate.Email;
                studentToUpdate.GroupId = studentToUpdate.GroupId;
                studentToUpdate.UserId = studentToUpdate.UserId;
                studentToUpdate.Password = studentToUpdate.Password;
            }
            await context.SaveChangesAsync();
        }
    }
}
