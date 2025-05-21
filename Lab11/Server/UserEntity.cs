using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server
{
    public class UserEntity : ITableEntity
    {
        public UserEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        public UserEntity() { }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Guid SessionId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}