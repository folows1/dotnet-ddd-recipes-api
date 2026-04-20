using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Application.UseCases.Login.ExternalLogin;

public class ExternalLoginUseCase : IExternalLoginUseCase
{
    private readonly IUserReadOnlyRepo _userReadOnlyRepo;
    private readonly IUserWriteOnlyRepo _userWriteOnlyRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public ExternalLoginUseCase(IUserReadOnlyRepo userReadOnlyRepo, IUserWriteOnlyRepo userWriteOnlyRepo,
        IUnitOfWork unitOfWork, IAccessTokenGenerator accessTokenGenerator)
    {
        _userReadOnlyRepo = userReadOnlyRepo;
        _userWriteOnlyRepo = userWriteOnlyRepo;
        _unitOfWork = unitOfWork;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<string> Execute(string name, string email)
    {
        var user = await _userReadOnlyRepo.GetByEmail(email);

        if (user is not null) return _accessTokenGenerator.Generate(user.UserIdentifier);

        user = new Domain.Entities.User
        {
            Email = email,
            Name = name,
            Password = "-"
        };

        await _userWriteOnlyRepo.Add(user);
        await _unitOfWork.Commit();

        return _accessTokenGenerator.Generate(user.UserIdentifier);
    }
}