namespace models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? HashPassword { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}