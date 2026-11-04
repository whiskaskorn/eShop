using System.Security.Claims;
using AppDbContext;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using models;
using Product_DTOs;

namespace ProductService;

public class ProductServices
{
    public static async Task<IResult> GetAll(UserContext db)
    {
        var products = await db.Products.ToListAsync();
        if(products == null)
        {
            return Results.BadRequest("Нема товарів!");
        }
        return Results.Ok(products);
    }
    public static async Task<IResult> AddProduct(PostProductDTO dto, IValidator<PostProductDTO> validator,UserContext db, ClaimsPrincipal currentUser)
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

            var product = new Product
            {
                Name = dto.Name,
                Decription = dto.Decription,
                Price = dto.Price,
                isActive = dto.isActive,
                UserId = user.Id
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Results.Created();
    }

    public static async Task<IResult> PutProduct(int id, PutProductDTO dto, UserContext db, ClaimsPrincipal currentUser)
    {
        var product = await db.Products.FindAsync(id);
        if(product == null)
        {
            return Results.NotFound("Товар не знайден!");
        }
        var idUserFromTokenString = currentUser.FindFirst("Id")?.Value;
        if (idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);
        if(product.UserId != idUserFromTokenInt)
        {
            return Results.BadRequest("Ви можете змінити лише свій товар!");
        }

        product.Name = dto.Name;
        product.Decription = dto.Decription;
        product.Price = dto.Price;
        product.isActive = dto.isActive;

        await db.SaveChangesAsync();
        return Results.Ok();
    }

    public static async Task<IResult> DeleteProduct(int id, UserContext db, ClaimsPrincipal currentUser)
    {
        var product = await db.Products.FindAsync(id);
        if(product == null)
        {
            return Results.NotFound("Товар не знайден!");
        }
        var idUserFromTokenString = currentUser.FindFirst("Id")?.Value;
        if (idUserFromTokenString == null)
        {
            return Results.Forbid();
        }
        int idUserFromTokenInt = Convert.ToInt32(idUserFromTokenString);

        if(product.UserId != idUserFromTokenInt)
        {
            return Results.BadRequest("Ви можете видалити лише свій товар!");
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}