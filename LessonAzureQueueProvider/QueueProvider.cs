using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonContracts;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types
using Newtonsoft.Json;

// https://docs.microsoft.com/en-us/azure/storage/queues/storage-dotnet-how-to-use-queues

namespace LessonAzureQueueProvider
{
    public class QueueProvider : ILessonQueueProvider
    {
        private readonly CloudQueue _queue;
        private const string QUEUE_NAME = "lessons";

        public QueueProvider()
        {
            string connStr = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connStr);
            var queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference(QUEUE_NAME);
            _queue.CreateIfNotExists();
        }

        public Task DeleteAsync(object token)
        {

            var message = (CloudQueueMessage)token;
            return _queue.DeleteMessageAsync(message);
        }

        public Task CancelAsync(object token)
        {
            var message = (CloudQueueMessage)token;
            return _queue.UpdateMessageAsync(message, TimeSpan.Zero, MessageUpdateFields.Visibility);
        }

        public async Task<ILessonQueueMessage> DequeueAsync()
        {
            CloudQueueMessage item;
            do
            {
                item = await _queue.GetMessageAsync();
                await Task.Delay(1000);
            } while (item == null);
            var data = JsonConvert.DeserializeObject<LessonMessage>(item.AsString);
            var message = new LessonQueueMessage(
                                        item,
                                        item.DequeueCount,
                                        data);
            return message;
        }
    }
}
