using Microsoft.EntityFrameworkCore;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;

var builder = WebApplication.CreateBuilder(args);
string connectionString = "Server=msuniversity.ru,1450;Database=nfcattend;TrustServerCertificate=True;User Id=nfcattend;Password=nfcattend;";

// Add services to the container.
builder.Services.AddDbContextFactory<TagContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
