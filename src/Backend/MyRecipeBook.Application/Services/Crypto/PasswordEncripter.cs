using System;
using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Application.Services.Crypto;

public class PasswordEncripter(string hashKey)
{
  public string Encrypt(string pwd)
  {
    var newPwd = $"{pwd}{hashKey}";

    var bytes = Encoding.UTF8.GetBytes(newPwd);

    var hashBytes = SHA512.HashData(bytes);

    return StringBytes(hashBytes);
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
