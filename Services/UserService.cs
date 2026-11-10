using AppDbContext;
using FluentValidation;
using models;
using Users_DTOs;
using PasswordHasher;
using Microsoft.EntityFrameworkCore;
using JwtService;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;

namespace UserService;

public static class UserServices
{
    public static int GenerateCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999);
    }
    public static async Task SendMesage(RegistrationUserDTO dto,IConfiguration configuration, int code)
    {
        string messageText = $"Привіт, {dto.Username}! Будь ласка,підтвердіть свою пошту для подальшої реєстрації!";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Developer", "whiskaskorn@gmail.com"));
        message.To.Add(new MailboxAddress(dto.Username, dto.Email!));
        message.Subject = $"Code: {code}";
        message.Body = new TextPart("plain")
        {
            Text = messageText
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("whiskaskorn@gmail.com", configuration.GetValue<string>("ApiKey:PasswordForMailKit") 
                ?? throw new InvalidOperationException("Не заданий пароль для пошти!"));
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
    public static async Task<IResult> GetAll(UserContext db)
    {
        var users = await db.Users.ToListAsync();
        return Results.Ok(users);
    }

    public static async Task<IResult> GetOne(UserContext db, ClaimsPrincipal currentUser)
    {
        var idUserFromTokenString = currentUser.FindFirst("Id")?.Value;
        if (idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);
        var user = await db.Users.FindAsync(idUserFromTokenInt);
        if (user == null)
        {
            return Results.NotFound("Користувач не знайден!");
        }
        GetUserDTO dto = new GetUserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
        return Results.Ok(dto);
    }

    public static async Task<IResult> Registration(RegistrationUserDTO dto, [FromServices] IValidator<RegistrationUserDTO> validator, UserContext db, Password_Hasher _hasher, IConfiguration configuration)
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
        int code = GenerateCode();
        await SendMesage(dto,configuration, code);

        User user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            HashPassword = _hasher.Generate(dto.Password!),
            Code = code
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created();
    }

    public static async Task<IResult> ConfirmEmail(ConfirmEmailDTO dto, UserContext db)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if(user == null)
        {
            return Results.BadRequest("Користувач за такою поштою не знайдений!");
        }
        if(dto.Code != user.Code || user.Code == null)
        {
            return Results.Conflict("Неправильний код!");
        }
        user.isConfirmEmail = true;
        user.Code = null;
        await db.SaveChangesAsync();
        return Results.Ok("Ви підтвердили Email!");
    }
    public static async Task<IResult> Login(LoginUserDTO dto, [FromServices] IValidator<LoginUserDTO> validator, UserContext db, Password_Hasher hasher, JwtServices jwt)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Email == dto.Email);
        if (user == null)
        {
            return Results.Conflict("Користувача з таким іменем або поштой не існує!");
        }

        if (user.isConfirmEmail == false)
        {
            return Results.BadRequest("Ви не можете залогінитись поки не підтвердите Email!");
        }
        var verifyPass = hasher.Verify(dto.Password!, user.HashPassword!);
        if (!verifyPass)
        {
            return Results.Conflict("Неправильний пароль!");
        }

        var token = jwt.GenerateJwtToken(user);
        return Results.Ok(token);
    }
    public static async Task<IResult> Put(PutUserDTO dto, [FromServices] IValidator<PutUserDTO> validator, UserContext db, ClaimsPrincipal currentUser, Password_Hasher hasher)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var idUserFromTokenString = currentUser.FindFirst("Id")?.Value;
        if (idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);

        var user = await db.Users.FindAsync(idUserFromTokenInt);

        if (user == null)
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
        if (idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);

        var user = await db.Users.FindAsync(idUserFromTokenInt);
        if (user == null)
        {
            return Results.NotFound("Користувач не знайден!");
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}