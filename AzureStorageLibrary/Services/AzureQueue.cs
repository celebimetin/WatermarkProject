using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class AzureQueue
    {
        private readonly QueueClient _queueClient;

        public AzureQueue(string queueName)
        {
            _queueClient = new QueueClient(ConnectionStrings.AzureStorageConnectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }

        public async Task<QueueMessage> RetrieveNextMessageAsync()
        {
            QueueProperties properties = await _queueClient.GetPropertiesAsync();
            _queueClient.PeekMessage();
            if (properties.ApproximateMessagesCount > 0)
            {
                QueueMessage[] queueMessages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(1));
                if (queueMessages.Any())
                {
                    return queueMessages[0];
                }
            }
            return null;
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            await _queueClient.DeleteMessageAsync(messageId, popReceipt);
        }
    }
}