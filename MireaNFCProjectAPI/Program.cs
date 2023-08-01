using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;

var builder = WebApplication.CreateBuilder(args);
string connectionString = "Server=POMAHTIK-PC\\SQLEXPRESS;Database=NFCProject;Trusted_Connection=True;TrustServerCertificate=True;";

// Add services to the container.
builder.Services.AddDbContextFactory<TagContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();