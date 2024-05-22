using Azure;
using Azure.Data.Tables;

namespace ChatGptInjection.Abstractions.Models
{
    public class StorageMessageEntity : ITableEntity
    {
        public string RequestMessage { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public long Created { get; set; }
        public string ResponseStorageId { get; set; } = string.Empty;
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
