using AutoMapper;
using MyRecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtils.Mapper;

public class MapperBuilder
{
    public static IMapper Build()
    {
        return new AutoMapper.MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping());
        }).CreateMapper();
    }
}