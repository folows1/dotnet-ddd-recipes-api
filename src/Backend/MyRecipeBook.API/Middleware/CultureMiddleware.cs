using System.Globalization;
using Dapper;
using MyRecipeBook.Domain.Extensions;

namespace MyRecipeBook.API.Middleware;

public class CultureMiddleware(RequestDelegate next)
{
  public async Task Invoke(HttpContext context)
  {
    var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

    var requestCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();
    var culture = new CultureInfo("en");

    if (requestCulture.NotEmpty()
        && supportedLanguages.Exists(c => c.Name.Equals(requestCulture)))
    {
      culture = new CultureInfo(requestCulture);
    }

    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;

    await next(context);
  }

}
