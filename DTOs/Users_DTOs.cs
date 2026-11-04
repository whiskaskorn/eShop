namespace Users_DTOs;

public class GetUserDTO
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}
public class RegistrationUserDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginUserDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class PutUserDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}