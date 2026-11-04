namespace Product_DTOs;

public class PostProductDTO
{
    public string? Name { get; set; }
    public string? Decription { get; set; }
    public decimal Price { get; set; }
    public bool isActive { get; set; } = true;
}