using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqps://bdvlehqs:OGJ3DslVWX2Eo2WGMPiMvbTjy5Eu5S5p@ostrich.lmq.cloudamqp.com/bdvlehqs")
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.ExchangeDeclareAsync("topic_exchange", ExchangeType.Topic);

for (var i = 1; i <= 10; i++)
{
    var routingKey = i % 2 == 0 ? "abc.def" : "abc.xyz";
    var message = $"Message {i} on {routingKey}";
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync("topic_exchange",
        routingKey,
        false,
        body);
                
    Console.WriteLine($"[Publisher] Published '{message}' with routing key '{routingKey}'");
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();