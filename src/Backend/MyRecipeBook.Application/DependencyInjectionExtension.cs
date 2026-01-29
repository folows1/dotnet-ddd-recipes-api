using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.Services.AutoMapper;
using MyRecipeBook.Application.Services.Crypto;
using MyRecipeBook.Application.UseCases.User.Register;

namespace MyRecipeBook.Application;

public static class DependencyInjectionExtension
{
  public static void AddApplication(this IServiceCollection services, IConfiguration cfg)
  {
    AddPwdEncripter(services, cfg);
    AddAutoMapper(services);
    AddUseCases(services);
  }

  private static void AddAutoMapper(IServiceCollection services)
  {
    var autoMapper = new AutoMapper.MapperConfiguration(options =>
    {
      options.AddProfile(new AutoMapping());
    }).CreateMapper();

    services.AddScoped(option => autoMapper);
  }

  private static void AddUseCases(IServiceCollection services)
  {
    services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
  }

  private static void AddPwdEncripter(IServiceCollection services, IConfiguration cfg)
  {
    var key = cfg.GetValue<string>("Settings:Password:AdditionalKey");

    services.AddScoped(option => new PasswordEncripter(key!));
  }
}
