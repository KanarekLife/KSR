using Azure.Data.Tables;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
    public class Service : IService
    {
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private const string TableName = "users";
        private const string PartitionKey = "users";
        private const string BlobContainerName = "userfiles";

        public bool Create(string login, string password)
        {
            var client = GetTableClient();

            var currentUser = client.GetEntityIfExists<UserEntity>(PartitionKey, login);

            if (currentUser.HasValue)
            {
                return false;
            }

            var user = new UserEntity(PartitionKey, login)
            {
                Login = login,
                Password = password,
                SessionId = Guid.Empty
            };
            client.AddEntity(user);
            return true;
        }

        public string Get(string name, Guid sessionId)
        {
            var userClient = GetTableClient();
            var sessionUser = userClient.Query<UserEntity>(e => e.SessionId == sessionId).FirstOrDefault();
            if (sessionUser == null)
            {
                return string.Empty;
            }
            var blobName = $"{sessionUser.Login}/{name}";
            var blobClient = GetBlobClient(blobName);
            if (!blobClient.Exists())
            {
                return string.Empty;
            }
            
            using (var stream = new System.IO.MemoryStream())
            {
                blobClient.DownloadTo(stream);
                stream.Position = 0;
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public Guid Login(string login, string password)
        {
            var client = GetTableClient();

            var currentUser = client.GetEntityIfExists<UserEntity>(PartitionKey, login);

            if (!currentUser.HasValue || currentUser.Value.Password != password)
            {
                return Guid.Empty;
            }
            
            var newSessionId = Guid.NewGuid();
            currentUser.Value.SessionId = newSessionId;
            client.UpdateEntity(currentUser.Value, currentUser.Value.ETag);
            return newSessionId;
        }

        public bool Logout(string login)
        {
            var client = GetTableClient();

            var currentUser = client.GetEntityIfExists<UserEntity>(PartitionKey, login);

            if (!currentUser.HasValue)
            {
                return false;
            }

            currentUser.Value.SessionId = Guid.Empty;
            client.UpdateEntity(currentUser.Value, currentUser.Value.ETag);

            return true;
        }

        public bool Put(string name, string content, Guid sessionId)
        {
            var tableClient = GetTableClient();

            var sessionUser = tableClient.Query<UserEntity>(e => e.SessionId == sessionId).FirstOrDefault();
            if (sessionUser == null)
            {
                return false;
            }

            var blobName = $"{sessionUser.Login}/{name}";
            var blobClient = GetBlobClient(blobName);

            using (var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                blobClient.Upload(stream, true);
            }
            return true;
        }

        private static TableClient GetTableClient(string tableName = TableName)
        {
            var client = new TableClient(ConnectionString, tableName);
            client.CreateIfNotExists();
            return client;
        }

        private static BlobClient GetBlobClient(string blobName, string containerName = BlobContainerName)
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();
            return containerClient.GetBlobClient(blobName);
        }
    }
}
