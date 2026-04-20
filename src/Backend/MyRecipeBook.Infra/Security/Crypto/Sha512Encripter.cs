using System.Security.Cryptography;
using System.Text;
using MyRecipeBook.Domain.Security.Cryptography;

namespace MyRecipeBook.Infra.Security.Crypto;

public class Sha512Encripter(string hashkey) : IPasswordEncripter
{
    public string Encrypt(string password)
    {
        var newPwd = $"{password}{hashkey}";

        var bytes = Encoding.UTF8.GetBytes(newPwd);

        var hashBytes = SHA512.HashData(bytes);

        return StringBytes(hashBytes);
    }

    public bool IsValid(string password, string passwordHash)
    {
        throw new NotImplementedException();
    }

    private static string StringBytes(byte[] bytes)
    {
        var sb = new StringBuilder();

        foreach (var b in bytes)
        {
            var hex = b.ToString("x2");
            sb.Append(hex);
        }

        return sb.ToString();
    }
}