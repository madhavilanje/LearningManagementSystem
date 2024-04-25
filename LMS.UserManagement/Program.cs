using LMS.Application.Implementations;
using LMS.UserManagement.Api.Extensions;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using LMS.Application.Interfaces;
using LMS.Infrastructure.Common.Interfaces;
using LMS.Infrastructure.SqlClietImplementations;
using LMS.Infrastructure.SqlClietInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using LMS.Infrastructure.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LMS.Infrastructure.SqlClientImplementations;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
// Add services to the container.

builder.Services.Configure<JsonSerializerSettings>(options =>
{
    options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.ConfigureAutoMapper();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUsersManager, UsersManager>();
builder.Services.AddTransient<ILearningPathsManager, LearningPathsManager>();
builder.Services.AddTransient<IEnrollmentsManager, EnrollmentsManager>();

builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
//Configure infrastructure services
builder.Services.AddDbContext<IUserDbContext, UserDbContext>(opts =>
{
opts.UseSqlServer(builder.Configuration.GetConnectionString("LMSConnectionString"));
});
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Enable CORS
app.UseCors(c =>
{
    c.AllowAnyHeader();
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
});

app.Run();
