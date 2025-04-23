using MassTransit;
using Messages;

var handler = new Handler();

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");

    sbc.ReceiveEndpoint(
        "ksr-b",
        ec =>
        {
            ec.Instance(handler);
        }
    );
});

await bus.StartAsync();
Console.WriteLine("[B] Press Enter to quit");

Console.ReadKey();
await bus.StopAsync();

internal class Handler : IConsumer<IMessage3>
{
    private int _counter;

    public Task Consume(ConsumeContext<IMessage3> context)
    {
        var headers = string.Join(
            ", ",
            context.Headers.GetAll().Select(header => $"{header.Key}: {header.Value}")
        );
        Console.WriteLine(
            $"[B] Received IMessage3 [Text1={context.Message.Text1}, Text2={context.Message.Text2}] with headers: {headers}, counter: {++_counter}"
        );
        return Task.CompletedTask;
    }
}
