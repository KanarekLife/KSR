using MassTransit;
using Messages;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");

    cfg.ReceiveEndpoint(
        "ksr-a",
        ec =>
        {
            ec.Handler<IMessage1>(Handle);
        }
    );
});

await bus.StartAsync();
Console.WriteLine("[A] Press Enter to quit");

Console.ReadKey();
await bus.StopAsync();
return;

static Task Handle(ConsumeContext<IMessage1> context)
{
    var headers = string.Join(
        ", ",
        context.Headers.GetAll().Select(header => $"{header.Key}: {header.Value}")
    );
    Console.WriteLine(
        $"[A] Received IMessage1 [Text1={context.Message.Text1}] with headers: {headers}"
    );
    return Task.CompletedTask;
}
