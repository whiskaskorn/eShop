namespace models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Decription { get; set; }
    public decimal Price { get; set; }
    public bool isActive { get; set; } = true;
    public int UserId { get; set; }
    public User? User { get; set; }
}