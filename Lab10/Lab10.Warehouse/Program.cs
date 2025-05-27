using Lab10.Warehouse;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<InventoryService>();
        
        services.AddMassTransit(config =>
        {
            config.AddConsumer<InventoryRequestConsumer>();
            config.AddConsumer<OrderAcceptedConsumer>();
            config.AddConsumer<OrderRejectedConsumer>();
            
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
                cfg.ReceiveEndpoint("warehouse", e =>
                {
                    e.ConfigureConsumer<InventoryRequestConsumer>(context);;
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

var busControl = host.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

Console.WriteLine($"Warehouse service started. Press 'A' to add items to warehouse, 'L' to display number of items or 'Q' to quit.");

while (true)
{
    var key = Console.ReadKey(true).Key;
    
    if (key == ConsoleKey.Q)
    {
        break;
    }
    
    if (key == ConsoleKey.A)
    {
        Console.Write("Enter quantity for the order: ");
        if (int.TryParse(Console.ReadLine(), out var quantity) && quantity > 0)
        {
            host.Services.GetRequiredService<InventoryService>().AddInventory(quantity);
            Console.WriteLine($"Added {quantity} items.");
        }
        else
        {
            Console.WriteLine("Invalid quantity. Please enter a positive number.");
        }
    }

    if (key == ConsoleKey.L)
    {
        var (available, reserved) = host.Services.GetRequiredService<InventoryService>().GetInventoryStatus();
        Console.WriteLine($"Available: {available}, Reserved: {reserved}");
    }
}

await busControl.StopAsync();

