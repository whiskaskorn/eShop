using Microsoft.EntityFrameworkCore;
using models;

namespace AppDbContext;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
}
