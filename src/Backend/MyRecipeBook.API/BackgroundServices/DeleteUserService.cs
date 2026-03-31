using Azure.Messaging.ServiceBus;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using MyRecipeBook.Infra.Services.ServiceBus;

namespace MyRecipeBook.API.BackgroundServices;

public class DeleteUserService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusProcessor _processor;

    public DeleteUserService(IServiceProvider serviceProvider, DeleteUserProcessor processor)
    {
        _serviceProvider = serviceProvider;
        _processor = processor.GetProcessor();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageAsync;

        _processor.ProcessErrorAsync += ExceptionReceivedHandler;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var msg = args.Message.Body.ToString();

        var userIdentifier = Guid.Parse(msg);

        var scope = _serviceProvider.CreateScope();

        var deleteUseCase = scope.ServiceProvider.GetRequiredService<IDeleteUserAccountUseCase>();

        await deleteUseCase.Execute(userIdentifier);
    }

    private static Task ExceptionReceivedHandler(ProcessErrorEventArgs _) => Task.CompletedTask;

    ~DeleteUserService() => Dispose();

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}