using MassTransit;
using Messages;

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");

    sbc.ReceiveEndpoint(
        "ksr-c",
        ec =>
        {
            ec.Handler<IMessage2>(Handle);
        }
    );
});

await bus.StartAsync();
Console.WriteLine("[C] Press Enter to quit");

Console.ReadKey();
await bus.StopAsync();
return;

static Task Handle(ConsumeContext<IMessage2> context)
{
    var headers = string.Join(
        ", ",
        context.Headers.GetAll().Select(header => $"{header.Key}: {header.Value}")
    );
    Console.WriteLine(
        $"[C] Received IMessage2 [Text2={context.Message.Text2}] with headers: {headers}"
    );
    return Task.CompletedTask;
}
