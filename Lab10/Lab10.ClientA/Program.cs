using Contracts;
using Lab10.ClientA;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string clientId = "A";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumer<ConfirmationRequestConsumer>();
            config.AddConsumer<OrderAcceptedConsumer>();
            config.AddConsumer<OrderRejectedConsumer>();

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");

                cfg.ReceiveEndpoint($"client-{clientId}", e =>
                {
                    e.ConfigureConsumer<ConfirmationRequestConsumer>(context);
                    e.ConfigureConsumer<OrderAcceptedConsumer>(context);
                    e.ConfigureConsumer<OrderRejectedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();
    
var busControl = host.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

Console.WriteLine($"Client {clientId} started. Press 'P' to place a new order or 'Q' to quit.");

while (true)
{
    var key = Console.ReadKey(true).Key;
    
    if (key == ConsoleKey.Q)
    {
        break;
    }
    
    if (key == ConsoleKey.P)
    {
        Console.Write("Enter quantity for the order: ");
        if (int.TryParse(Console.ReadLine(), out var quantity) && quantity > 0)
        {
            var sendEndpoint = await busControl.GetPublishSendEndpoint<StartOrder>();
            await sendEndpoint.Send(new StartOrder 
            { 
                Quantity = quantity,
                ClientId = clientId
            });
            Console.WriteLine($"Order for {quantity} items placed.");
        }
        else
        {
            Console.WriteLine("Invalid quantity. Please enter a positive number.");
        }
    }
}

await busControl.StopAsync();
