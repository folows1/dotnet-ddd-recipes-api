using Microsoft.AspNetCore.Http;

namespace MyRecipeBook.Communication.Requests;

public class RequestRecipeFormData : RequestRecipeJson
{
    public IFormFile? Image { get; set; }
}