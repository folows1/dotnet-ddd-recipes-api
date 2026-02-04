namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException(IList<string> errors) : MyRecipeBookException(string.Empty)
{
    public IList<string> ErrorMessages { get; set; } = errors;
}