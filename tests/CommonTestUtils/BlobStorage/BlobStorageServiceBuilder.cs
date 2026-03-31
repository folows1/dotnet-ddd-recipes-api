using Bogus;
using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace CommonTestUtils.BlobStorage;

public class BlobStorageServiceBuilder
{
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock = new();

    public BlobStorageServiceBuilder GetImageUrl(User user, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return this;

        var faker = new Faker();
        var imgUrl = faker.Image.LoremPixelUrl();

        _blobStorageServiceMock.Setup(blob => blob.GetImageUrl(user, fileName)).ReturnsAsync(imgUrl);

        return this;
    }

    public BlobStorageServiceBuilder GetImageUrl(User user, IList<Recipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            GetImageUrl(user, recipe.ImageIdentifier);
        }

        return this;
    }


    public IBlobStorageService Build() => _blobStorageServiceMock.Object;
}