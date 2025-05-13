using Lab10.Shop;
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
    
Console.WriteLine("Store service started. Press Ctrl+C to exit.");
await host.RunAsync();
