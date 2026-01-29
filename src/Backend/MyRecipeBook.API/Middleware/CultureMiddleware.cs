using System.Globalization;

namespace MyRecipeBook.API.Middleware;

public class CultureMiddleware
{
  private readonly RequestDelegate _next;

  public CultureMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context)
  {
    var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures);

    var requestCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();
    var culture = new CultureInfo("en");

    if (string.IsNullOrWhiteSpace(requestCulture) == false
        && supportedLanguages.Any(c => c.Name.Equals(requestCulture)))
    {
      culture = new CultureInfo(requestCulture);
    }

    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;

    await _next(context);
  }

}
