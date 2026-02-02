using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MyRecipeBook.Application.Services.Crypto;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase(
  IUserWriteOnlyRepo writeOnlyRepo,
  IUserReadOnlyRepo readOnlyRepo,
  IUnitOfWork unitOfWork,
  IMapper mapper,
  PasswordEncripter pwdEncripter)
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

    await writeOnlyRepo.Add(user);

    await unitOfWork.Commit();

    return new ResponseRegisteredUserJson
    {
      Name = user.Name,
    };
  }

  private async Task Validate(RequestRegisterUserJson request)
  {
    var validator = new RegisterUserValidator();

    var result = await validator.ValidateAsync(request);

    var emailExist = await readOnlyRepo.ExistActiveUserWithEmail(request.Email);

    if (emailExist)
    {
      result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.NAME_EMPTY));
    }

    if (result.IsValid.IsFalse())
    {
      var errMsgs = result.Errors.Select(e => e.ErrorMessage).ToList();
      throw new ErrorOnValidationException(errMsgs);
    }
  }
}
