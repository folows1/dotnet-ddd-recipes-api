using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test;

public class MyRecipeBookClassFixture(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    protected async Task<HttpResponseMessage> DoPost(string method, object request, string culture = "en")
    {
        ChangeRequestCulture(culture);
        return await _client.PostAsJsonAsync(method, request);
    }

    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);
        return await _client.GetAsync(method);
    }

    private void ChangeRequestCulture(string culture)
    {
        if (_client.DefaultRequestHeaders.Contains("Accept-Language"))
            _client.DefaultRequestHeaders.Remove("Accept-Language");

        _client.DefaultRequestHeaders.Add("Accept-Language", culture);
    }

    private void AuthorizeRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}