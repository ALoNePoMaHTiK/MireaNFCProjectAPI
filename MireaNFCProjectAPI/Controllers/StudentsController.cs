using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;
using System.Security.Cryptography;
using System.Text;

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

        [HttpGet("ByIsAcceptRequested/{isAcceptRequested}")]
        public async Task<IEnumerable<Student>> GetByIsAcceptRequested(bool isAcceptRequested)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            return await context.Students.Where(s => s.IsAcceptRequested == isAcceptRequested).ToListAsync();
        }

        [HttpPost("Auth/")]
        public async Task<ActionResult<Student>> GetByAuth([FromBody] AuthData authData)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            Student s = await context.Students.Where(s => s.Email == authData.Email).FirstOrDefaultAsync();
            string hash = "";
            if (s != null)
                hash = GetHash(authData.Password);
            if (s == null || s.Password != hash)
            {
                return new NotFoundResult();
            }    
            else
            {
                s.IsAccepted = false;
                s.IsAcceptRequested = true;
                await context.SaveChangesAsync();
                return new OkObjectResult(s);
            }    
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
            student.Password = GetHash(student.Password);
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
                studentToUpdate.Email = student.Email;
                studentToUpdate.GroupId = student.GroupId;
                studentToUpdate.UserId = student.UserId;
                studentToUpdate.Password = student.Password;
                studentToUpdate.IsAccepted = student.IsAccepted;
                studentToUpdate.IsAcceptRequested = student.IsAcceptRequested;
            }
            await context.SaveChangesAsync();
        }

        static private string GetHash(string message)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashValue = sha512.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToHexString(hashValue);
            }
        }
    }
}
