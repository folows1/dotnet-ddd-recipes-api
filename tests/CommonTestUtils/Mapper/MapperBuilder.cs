using AutoMapper;
using CommonTestUtils.Crypto;
using MyRecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtils.Mapper;

public class MapperBuilder
{
    public static IMapper Build()
    {
        var sqIds = IdEncripterBuilder.Build();

        return new MapperConfiguration(options => { options.AddProfile(new AutoMapping(sqIds)); })
            .CreateMapper();
    }
}