using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using System;

namespace Server
{
    public class Service : IService
    {
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private const string BlobContainerName = "input";
        private const string EncodedBlobContainerName = "encoded";
        private const string QueueName = "zadania";

        public void Koduj(string nazwa, string tresc)
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(BlobContainerName);
            containerClient.CreateIfNotExists();

            var blobClient = containerClient.GetBlobClient(nazwa);
            blobClient.Upload(BinaryData.FromString(tresc), overwrite: true);

            var queueClient = new QueueClient(ConnectionString, QueueName);
            queueClient.CreateIfNotExists();
            queueClient.SendMessage(nazwa);
        }

        public string Pobierz(string nazwa)
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(EncodedBlobContainerName);
            containerClient.CreateIfNotExists();

            var blobClient = containerClient.GetBlobClient(nazwa);

            if (!blobClient.Exists())
                return null;

            var data = blobClient.DownloadContent();
            return data.Value.Content.ToString();
        }
    }
}
