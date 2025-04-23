using MassTransit;
using Messages;
using Publisher;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
});

await bus.StartAsync();

for (var i = 0; i < 10; i++)
{
    IMessage1 message1 = new Message1 { Text1 = $"Message 1 #{i}" };
    var headers1 = new Dictionary<string, string>
    {
        { "id", i.ToString() },
        { "hash", message1.GetHashCode().ToString() },
        { "receiver", "Subscriber A, Subscriber B" },
    };
    await SendMessage(message1, headers1);

    IMessage2 message2 = new Message2 { Text2 = $"Message 2 #{i}" };
    var headers2 = new Dictionary<string, string>
    {
        { "id", i.ToString() },
        { "hash", message2.GetHashCode().ToString() },
        { "receiver", "Subscriber B, Subscriber C" },
    };
    await SendMessage(message2, headers2);

    IMessage3 message3 = new Message3 { Text1 = $"Message 3 #{i}", Text2 = $"Message 3 #{i}" };
    var headers3 = new Dictionary<string, string>
    {
        { "id", i.ToString() },
        { "hash", message3.GetHashCode().ToString() },
        { "receiver", "Subscriber A, Subscriber B, Subscriber C" },
    };
    await SendMessage(message3, headers3);
    await Task.Delay(100);
}

return;

async Task SendMessage<T>(T message, Dictionary<string, string> headers)
{
    var messageHeaders = string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}"));

    Console.WriteLine($"[Publisher] Sending [{message}] with headers [{messageHeaders}]");

    await bus.Publish(
        message!,
        context =>
        {
            foreach (var (key, value) in headers)
            {
                context.Headers.Set(key, value);
            }
        }
    );
}
