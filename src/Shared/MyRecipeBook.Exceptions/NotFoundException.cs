using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Exceptions;

public class NotFoundException : MyRecipeBookException
{
    public NotFoundException(string message) : base(message)
    {
    }
}