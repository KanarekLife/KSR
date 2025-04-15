using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqps://bdvlehqs:OGJ3DslVWX2Eo2WGMPiMvbTjy5Eu5S5p@ostrich.lmq.cloudamqp.com/bdvlehqs")
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.ExchangeDeclareAsync("topic_exchange", ExchangeType.Topic);

var queueName = (await channel.QueueDeclareAsync()).QueueName;
await channel.QueueBindAsync(queueName, "topic_exchange", "*.xyz");
Console.WriteLine("[Subscriber2] Waiting for messages. Subscribing to channels ending with 'xyz'. Press any key to exit...");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var text = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[Subscriber2] Received \"{text}\" with routing key '{ea.RoutingKey}'");
    return Task.CompletedTask;
};
await channel.BasicConsumeAsync(queueName, true, consumer);
Console.ReadLine();
