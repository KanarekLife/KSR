using Lab10.Shop;
using MassTransit;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(config =>
        {
            config.AddPublishMessageScheduler();
            config.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .InMemoryRepository();
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
                cfg.UseDelayedMessageScheduler();
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();
    
Console.WriteLine("Store service started. Press Ctrl+C to exit.");
await host.RunAsync();