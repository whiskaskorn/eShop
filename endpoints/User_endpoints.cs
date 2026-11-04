using AppDbContext;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using models;
using UserService;

namespace Users_DTOs;

public static class UserEndpoints
{
    public static void MapUserEndpoints(WebApplication app)
    {
        var userGroup = app.MapGroup("/users").RequireAuthorization();
        var authGroup = app.MapGroup("/users");


        userGroup.MapGet("/", UserServices.GetAll).AllowAnonymous();
        userGroup.MapGet("/me", UserServices.GetOne);
        authGroup.MapPost("/registration", UserServices.Registration);
        authGroup.MapPost("/login", UserServices.Login);
        userGroup.MapPut("/me", UserServices.Put);
        userGroup.MapDelete("/me", UserServices.Delete);
    }
}