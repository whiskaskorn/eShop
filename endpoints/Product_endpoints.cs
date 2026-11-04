using AppDbContext;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using models;
using Product_Validator;
using ProductService;
using UserService;

namespace Product_DTOs;

public class ProductEndpoints
{
    public static void MapProductEndpoints(WebApplication app)
    {
        var productsGroup = app.MapGroup("/products").RequireAuthorization();
        productsGroup.MapGet("/",ProductServices.GetAll).AllowAnonymous();
        productsGroup.MapPost("/", ProductServices.AddProduct);
        productsGroup.MapPut("/{id}",ProductServices.PutProduct);
        productsGroup.MapDelete("/{id}",ProductServices.DeleteProduct);
    }
}