using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqps://bdvlehqs:OGJ3DslVWX2Eo2WGMPiMvbTjy5Eu5S5p@ostrich.lmq.cloudamqp.com/bdvlehqs")
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
var replyQueueName = (await channel.QueueDeclareAsync()).QueueName;
var replyConsumer = new AsyncEventingBasicConsumer(channel);
replyConsumer.ReceivedAsync += (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var text = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[Sender] Received Reply: {text}");
    return Task.CompletedTask;
};
await channel.BasicConsumeAsync(queue: replyQueueName, autoAck: true, consumer: replyConsumer);
for (var i = 1; i <= 10; i++)
{
    var body = Encoding.UTF8.GetBytes($"Message {i}");
    var properties = new BasicProperties
    {
        Persistent = true,
        ReplyTo = replyQueueName,
        Headers = new Dictionary<string, object>
        {
            { "no", i.ToString() },
            { "sentOnUtc", DateTime.UtcNow.ToString("s") },
        }!
    };
    await channel.BasicPublishAsync(string.Empty, "task_queue", false, properties, body);
    Console.WriteLine($"[Sender] Sent Message: \"Message {i}\"");
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
