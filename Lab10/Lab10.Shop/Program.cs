using Lab10.Shop;
using MassTransit;
using Microsoft.Extensions.Hosting;

var repo = new InMemorySagaRepository<OrderState>();
var machine = new OrderStateMachine();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
                cfg.ReceiveEndpoint("OrderState", e =>
                {
                    e.StateMachineSaga(machine, repo);
                });
                cfg.UseInMemoryScheduler();
            });
        });
    })
    .Build();
    
Console.WriteLine("Store service started. Press Ctrl+C to exit.");
await host.RunAsync();