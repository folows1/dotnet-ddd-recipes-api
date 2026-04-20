using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException(IList<string> errorMessages) : MyRecipeBookException(string.Empty)
{
    public override IList<string> GetErrorMessages() => errorMessages;

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
}