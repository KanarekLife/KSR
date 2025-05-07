using MassTransit;
using Messages;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
    cfg.ReceiveEndpoint("b_recv_queue", conf =>
    {
        conf.Handler<Publ>(ctx =>
        {
            if (ctx.Message.Number % 2 != 0)
            {
                return Task.CompletedTask;
            }

            ctx.RespondAsync(new OdpB("B"));
            Console.WriteLine($"[B] Received: {ctx.Message}");
            return Task.CompletedTask;
        });
    });
    cfg.ReceiveEndpoint("b_recv_queue_error", conf =>
    {
        conf.Handler<Fault>(ctx =>
        {
            foreach (var ex in ctx.Message.Exceptions)
            {
                Console.WriteLine($"[B] Error: {ex.Message}");
            }
            return Task.CompletedTask;
        });
    });
});

await bus.StartAsync();

Console.WriteLine($"[A] Press 'enter' to exit.");
Console.ReadLine();
