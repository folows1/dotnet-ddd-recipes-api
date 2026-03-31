using Azure.Messaging.ServiceBus;

namespace MyRecipeBook.Infra.Services.ServiceBus;

public class DeleteUserProcessor(ServiceBusProcessor processor)
{
    public ServiceBusProcessor GetProcessor()
    {
        return processor;
    }
}