using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sqids;

namespace MyRecipeBook.API.Binders;

public class MyRecipeBookIdBinder : IModelBinder
{
    private readonly SqidsEncoder<long> _encoder;

    public MyRecipeBookIdBinder(SqidsEncoder<long> encoder) => _encoder = encoder;

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(value))
            return Task.CompletedTask;

        var id = _encoder.Decode(value).Single();

        bindingContext.Result = ModelBindingResult.Success(id);
        return Task.CompletedTask;
    }
}