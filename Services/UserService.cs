using AppDbContext;
using FluentValidation;
using models;
using Users_DTOs;
using PasswordHasher;
using Microsoft.EntityFrameworkCore;
using JwtService;
using System.Security.Claims;

namespace UserService;

public static class UserServices
{
    public static async Task<IResult> GetAll(UserContext db)
    {
        var users = await db.Users.ToListAsync();
        return Results.Ok(users);
    }
    public static async Task<IResult> Registration(RegistrationUserDTO dto, IValidator<RegistrationUserDTO> validator, UserContext db,Password_Hasher _hasher)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var userIsExsists = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
        if (userIsExsists != null)
        {
            return Results.Conflict("Користувач з таким іменем або поштой вже існує!");
        }

        User user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            HashPassword = _hasher.Generate(dto.Password!)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created();
    }

    public static async Task<IResult> Login(LoginUserDTO dto, IValidator<LoginUserDTO> validator,UserContext db, Password_Hasher hasher,JwtServices jwt)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Email == dto.Email);
        if(user == null)
        {
            return Results.Conflict("Користувача з таким іменем або поштой не існує!");
        }

        var verifyPass = hasher.Verify(dto.Password!,user.HashPassword!);
        if (!verifyPass)
        {
            return Results.Conflict("Неправильний пароль!");
        }

        var token = jwt.GenerateJwtToken(user);
        return Results.Ok(token);
    }
    public static async Task<IResult> Put(PutUserDTO dto, IValidator<PutUserDTO> validator,UserContext db, ClaimsPrincipal currentUser, Password_Hasher hasher)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var idUserFromTokenString = currentUser.FindFirst("Id")?.Value;
        if(idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);

        var user = await db.Users.FindAsync(idUserFromTokenInt);

        if(user == null)
        {
            return Results.NotFound("Користувач не знайден!");
        }

        bool successPass = hasher.Verify(dto.CurrentPassword!, user.HashPassword!);
        if (!successPass)
        {
            return Results.BadRequest("Невірний пароль!");
        }

        var userIsExsists = await db.Users.FirstOrDefaultAsync(u => u.Id != idUserFromTokenInt && (u.Username == dto.Username || u.Email == dto.Email));
        if (userIsExsists != null)
        {
            return Results.Conflict("Користувач з таким іменем або поштой вже існує!");
        }

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.HashPassword = hasher.Generate(dto.NewPassword!);

        await db.SaveChangesAsync();
        return Results.Ok("Користувач оновлен!");
    }
    public static async Task<IResult> Delete(UserContext db, ClaimsPrincipal currentUser)
    {
        var idUserFromTokenString = currentUser.FindFirst("Id")?.Value;
        if(idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);

        var user = await db.Users.FindAsync(idUserFromTokenInt);
        if(user == null)
        {
            return Results.NotFound("Користувач не знайден!");
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}