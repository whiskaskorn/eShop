using MailKit.Net.Smtp;
using System.Text;
using AppDbContext;
using AuthService;
using FluentValidation;
using JwtService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PasswordHasher;
using Product_DTOs;
using Users_DTOs;

Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<Password_Hasher>();
builder.Services.AddScoped<JwtServices>();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<UserContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:ConnectionPostGre"]));
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

UserEndpoints.MapUserEndpoints(app);
ProductEndpoints.MapProductEndpoints(app);


app.Run();