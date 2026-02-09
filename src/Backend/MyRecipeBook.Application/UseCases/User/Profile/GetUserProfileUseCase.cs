using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Services;

namespace MyRecipeBook.Application.UseCases.User.Profile;

public class GetUserProfileUseCase(ILoggedUser loggedUser, IMapper mapper) : IGetUserProfileUseCase
{
    public async Task<ResponseUserProfileJson> Execute()
    {
        var user = await loggedUser.User();

        return mapper.Map<ResponseUserProfileJson>(user);
    }
}