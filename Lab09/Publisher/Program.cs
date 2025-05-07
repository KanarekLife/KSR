using Common;
using MassTransit;
using MassTransit.Serialization;
using Messages;

var isRunning = true;
var statistics = new int[5];

var controllerBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseEncryptedSerializer(new AesCryptoStreamProvider(
        new SymmetricKeyProvider("19304419304419304419304419304419"), "1930441930441930"));
    cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
    cfg.ReceiveEndpoint("control_queue", conf =>
    {
        conf.Handler<Ustaw>(ctx =>
        {
            Console.WriteLine($"[P] Received: {ctx.Message}");
            isRunning = ctx.Message.Dziala;
            if (!isRunning)
            {
                Console.WriteLine("[P] Statistics:");
                Console.WriteLine($"\tA Attempts: {statistics[0]}");
                Console.WriteLine($"\tB Attempts: {statistics[1]}");
                Console.WriteLine($"\tA Successes: {statistics[2]}");
                Console.WriteLine($"\tB Successes: {statistics[3]}");
                Console.WriteLine($"\tSent: {statistics[4]}");
            }
            return Task.CompletedTask;
        });
    });
});
var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://bdvlehqs:e6ORNoNXeD7FG-9uubLxF5BuYLUXoOpW@ostrich.lmq.cloudamqp.com/bdvlehqs");
    cfg.ReceiveEndpoint("a_queue", conf =>
    {
        conf.UseMessageRetry(r => r.Immediate(5));
        conf.Handler<OdpA>(ctx =>
        {
            statistics[0]++;
            Console.WriteLine($"[P] Received A: {ctx.Message}");
            if (Random.Shared.Next(0, 3) == 0)
            {
                throw new Exception("TEST EXCEPTION");
            }
            statistics[2]++;
            return Task.CompletedTask;
        });
    });
    cfg.ReceiveEndpoint("b_queue", conf =>
    {
        conf.UseMessageRetry(r => r.Immediate(5));
        conf.Handler<OdpB>(ctx =>
        {
            statistics[1]++;
            Console.WriteLine($"[P] Received B: {ctx.Message}");
            if (Random.Shared.Next(0, 3) == 0)
            {
                throw new Exception("TEST EXCEPTION");
            }
            statistics[3]++;
            return Task.CompletedTask;
        });
    });
});

await controllerBus.StartAsync();
await bus.StartAsync();

do
{
    if (!isRunning)
    {
        continue;
    }

    var message = new Publ(statistics[4] + 1);
    Console.WriteLine($"[P] Sending: {message}");
    await bus.Publish(message);
    statistics[4]++;
} while (true);
