using AutoMapper;
using FluentValidation.Results;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.RefreshToken;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase(
    IUserWriteOnlyRepo writeOnlyRepo,
    IUserReadOnlyRepo readOnlyRepo,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IAccessTokenGenerator accessTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITokenRepo tokenRepo,
    IPasswordEncripter pwdEncripter)
    : IRegisterUserUseCase
{
    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        // var cryptoPwd = new PasswordEncripter();

        // var autoMapper = new AutoMapper.MapperConfiguration(options =>
        // {
        //   options.AddProfile(new AutoMapping());
        // }).CreateMapper();

        await Validate(request);

        var user = mapper.Map<Domain.Entities.User>(request);

        // AUTOMAPPER -> MAPSTER

        user.Password = pwdEncripter.Encrypt(request.Password);

        user.UserIdentifier = Guid.NewGuid();

        await writeOnlyRepo.Add(user);

        await unitOfWork.Commit();

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

    private async Task Validate(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserValidator();

        var result = await validator.ValidateAsync(request);

        var emailExist = await readOnlyRepo.ExistActiveUserWithEmail(request.Email);

        if (emailExist)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        }

        if (result.IsValid.IsFalse())
        {
            var errMsgs = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errMsgs);
        }
    }
}