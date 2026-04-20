using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.RefreshToken;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;

public class DoLoginUseCase(
    IUserReadOnlyRepo repo,
    IPasswordEncripter passwordEncripter,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITokenRepo tokenRepo,
    IUnitOfWork unitOfWork,
    IAccessTokenGenerator accessTokenGenerator)
    : IDoLoginUseCase
{
    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        // var pwd = passwordEncripter.Encrypt(request.Password);

        // var user = await repo.GetByEmailAndPassword(request.Email, pwd);
        var user = await repo.GetByEmail(request.Email);

        if (user is null ||
            passwordEncripter.IsValid(request.Password, user.Password).IsFalse())
            throw new InvalidLoginException();

        var refreshToken = await CreateAndSaveRefreshToken(user);

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Tokens = new ResponseTokensJson
            {
                RefreshToken = refreshToken,
                AccessToken = accessTokenGenerator.Generate(user.UserIdentifier),
            },
        };
    }

    private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
    {
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Value = refreshTokenGenerator.Generate(),
            UserId = user.Id
        };

        await tokenRepo.SaveNewRefreshToken(refreshToken);
        await unitOfWork.Commit();

        return refreshToken.Value;
    }
}