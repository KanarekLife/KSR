using Common;
using MassTransit;
using MassTransit.Serialization;
using Messages;

var controllerBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseEncryptedSerializer(new AesCryptoStreamProvider(
        new SymmetricKeyProvider("19304419304419304419304419304419"), "1930441930441930"));
    cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
});

Console.WriteLine($"[C] Press 's' to stop the program and 'r' to resume it.");

while (true)
{
    var key = Console.ReadKey();
    Ustaw? message = null;
    
    switch (key.Key)
    {
        case ConsoleKey.S:
            message = new Ustaw(false);
            Console.WriteLine("[C] Stopping the program.");
            break;
        case ConsoleKey.R:
            message = new Ustaw(true);
            Console.WriteLine("[C] Resuming the program.");
            break;
    }

    if (message is not null)
    {
        await controllerBus.Publish(message, ctx =>
        {
            ctx.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader, Guid.NewGuid().ToString());
        });
    }
}
