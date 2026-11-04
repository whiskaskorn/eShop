using BCrypt;
namespace PasswordHasher;
public class Password_Hasher
{
    public string Generate(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool Verify(string password,string hashPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password,hashPassword);
    }
}