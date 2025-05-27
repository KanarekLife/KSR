using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Worker
{
    // CMD: & "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\WcfTestClient.exe"
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        private const string StorageConnectionString = "UseDevelopmentStorage=true";
        private const string QueueName = "zadania";
        private const string BlobContainerName = "input";
        private const string EncodedBlobContainerName = "encoded";

        public override void Run()
        {
            Trace.TraceInformation("Worker is running");

            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 12;

            var result = base.OnStart();
            Trace.TraceInformation("Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Worker is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            Trace.TraceInformation("Worker logic started");

            var queueClient = new QueueClient(StorageConnectionString, QueueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobServiceClient = new BlobServiceClient(StorageConnectionString);
            var inputContainer = blobServiceClient.GetBlobContainerClient(BlobContainerName);
            var encodedContainer = blobServiceClient.GetBlobContainerClient(EncodedBlobContainerName);
            await inputContainer.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            await encodedContainer.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var rnd = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var msgResponse = await queueClient.ReceiveMessageAsync(cancellationToken: cancellationToken);
                    var msg = msgResponse.Value;
                    if (msg != null)
                    {
                        var nazwa = msg.Body.ToString();
                        var done = false;

                        while (!done && !cancellationToken.IsCancellationRequested)
                        {
                            try
                            {
                                var blobClient = inputContainer.GetBlobClient(nazwa);
                                if (!await blobClient.ExistsAsync(cancellationToken))
                                {
                                    Trace.TraceWarning($"Blob {nazwa} does not exist.");
                                    break;
                                }
                                var data = (await blobClient.DownloadContentAsync(cancellationToken)).Value.Content.ToString();
                                
                                if (rnd.Next(3) == 0)
                                {
                                    throw new Exception("Random ROT13 error!");
                                }
                                var encoded = Rot13(data);
                                
                                var encodedBlobClient = encodedContainer.GetBlobClient(nazwa);
                                await encodedBlobClient.UploadAsync(BinaryData.FromString(encoded), overwrite: true, cancellationToken: cancellationToken);

                                done = true;
                                Trace.TraceInformation($"Blob {nazwa} encoded and saved.");
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceWarning($"Error encoding blob {nazwa}: {ex.Message}. Retrying...");
                                await Task.Delay(1000, cancellationToken);
                            }
                        }
                        await queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, cancellationToken);
                    }
                    else
                    {
                        await Task.Delay(1000, cancellationToken);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Worker exception: {ex.Message}");
                    await Task.Delay(2000, cancellationToken);
                }
            }

            Trace.TraceInformation("Worker logic ended");
        }

        // ROT13 implementation
        private static string Rot13(string input)
        {
            var array = input.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                int c = array[i];
                if (c >= 'a' && c <= 'z')
                    array[i] = (char)('a' + (c - 'a' + 13) % 26);
                else if (c >= 'A' && c <= 'Z')
                    array[i] = (char)('A' + (c - 'A' + 13) % 26);
            }
            return new string(array);
        }
    }
}
