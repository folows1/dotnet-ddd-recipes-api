using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using MyRecipeBook.Application.Services.Crypto;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
  private readonly IUserWriteOnlyRepo _writeOnlyRepo;
  private readonly IUserReadOnlyRepo _readOnlyRepo;
  private readonly IMapper _mapper;
  private readonly PasswordEncripter _pwdEncripter;
  private readonly IUnitOfWork _unitOfWork;

  public RegisterUserUseCase(
    IUserWriteOnlyRepo writeOnlyRepo,
    IUserReadOnlyRepo readOnlyRepo,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    PasswordEncripter pwdEncripter
  )
  {
    _unitOfWork = unitOfWork;
    _readOnlyRepo = readOnlyRepo;
    _writeOnlyRepo = writeOnlyRepo;
    _mapper = mapper;
    _pwdEncripter = pwdEncripter;
  }

  public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
  {
    // var cryptoPwd = new PasswordEncripter();

    // var autoMapper = new AutoMapper.MapperConfiguration(options =>
    // {
    //   options.AddProfile(new AutoMapping());
    // }).CreateMapper();

    await Validate(request);

    var user = _mapper.Map<Domain.Entities.User>(request);

    // AUTOMAPPER -> MAPSTER

    user.Password = _pwdEncripter.Encrypt(request.Password);

    await _writeOnlyRepo.Add(user);

    await _unitOfWork.Commit();

    return new ResponseRegisteredUserJson
    {
      Name = request.Name,
    };
  }

  private async Task Validate(RequestRegisterUserJson request)
  {
    var validator = new RegisterUserValidator();

    var result = validator.Validate(request);

    var emailExist = await _readOnlyRepo.ExistActiveUserWithEmail(request.Email);

    if (emailExist)
    {
      result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.NAME_EMPTY));
    }

    if (result.IsValid == false)
    {
      var errMsgs = result.Errors.Select(e => e.ErrorMessage).ToList();
      throw new ErrorOnValidationException(errMsgs);
    }
  }
}
