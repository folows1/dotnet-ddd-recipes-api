using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;

public class RegisterRecipeUseCase(
    IRecipeWriteOnlyRepo repo,
    ILoggedUser user,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBlobStorageService blobStorageService
) : IRegisterRecipeUseCase
{
    public async Task<ResponseRegisterRecipeJson> Execute(RequestRecipeFormData request)
    {
        Validate(request);

        var loggedUser = await user.User();

        var recipe = mapper.Map<Domain.Entities.Recipe>(request);
        recipe.UserId = loggedUser.Id;

        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
        for (var index = 0; index < instructions.Count; index++)
            instructions[index].Step = index + 1;

        recipe.Instructions = mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        if (request.Image is not null)
        {
            var fileStream = request.Image.OpenReadStream();

            var (isValidImage, extension) = fileStream.ValidateAndGetImageExtension();

            if (isValidImage.IsFalse())
                throw new ErrorOnValidationException([ResourceMessagesException.TOKEN_INVALID]);

            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";
            await blobStorageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
        }

        await repo.Add(recipe);

        await unitOfWork.Commit();

        return mapper.Map<ResponseRegisterRecipeJson>(recipe);
    }

    private static void Validate(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
    }
}