using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Models;
using MireaNFCProjectAPI.Contexts;
using MireaNFCProjectAPI.Models;

var builder = WebApplication.CreateBuilder(args);
string connectionString = "Server=msuniversity.ru,1450;Database=nfcattend;TrustServerCertificate=True;User Id=nfcattend;Password=nfcattend;";

// Add services to the container.
builder.Services.AddDbContextFactory<TagContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<UserContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<GroupContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<StudentContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<RoomContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<CheckoutContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{{ 
            new OpenApiSecurityScheme(){
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            },
            In = ParameterLocation.Header
    }, new List<string>()}});
});

builder.Services.AddCors();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "MIREA NFCProject API - V1",
            Version = "v1"
        }
     );

    c.IncludeXmlComments("obj\\Debug\\net6.0\\MireaNFCProjectAPI.xml");
});

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
