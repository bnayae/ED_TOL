using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LessonContracts;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace LessonSimulator
{
    class Program
    {
        private const string QUEUE_NAME = "lessons";
        private static readonly Guid LOGIN_SESSION_ID = Guid.NewGuid();
        private static readonly TimeSpan DELAY = TimeSpan.FromSeconds(2);

        static void Main(string[] args)
        {
            string connStr = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connStr);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(QUEUE_NAME);
            queue.CreateIfNotExists();

            Task t = SimulateAsync(queue);
            t.Wait();
        }

        #region SimulateAsync

        private static async Task SimulateAsync(CloudQueue queue)
        {
            while (true)
            {

                Console.WriteLine(@"
+-------------------------------------------------+
|   ^        = tab                                |
|   L        = Lesson session identifier          |
|   #        = Start                              |
|   -|       = End                                |
|   #L1(^A)-| = Start Lesson 1 on Tab A, end it    |
+-------------------------------------------------+

1. #L1(^A)-|
2. #L1(^A)----------------|
           #L1(^B)-----|
");
                char c = Console.ReadKey(true).KeyChar;
                switch (c)
                {
                    case '1':
                        await SimulateStartStopAsync(queue);
                        break;
                    case '2':
                        await SimulateStartAndDuplicateTabAsync(queue);
                        break;
                    default:
                        Console.WriteLine("\r\nInvalid choice");
                        break;
                }
            }
        }

        #endregion // SimulateAsync

        #region SimulateStartStopAsync

        //  #L1(^A)-|
        private static async Task SimulateStartStopAsync(CloudQueue queue)
        {
            #region #L1(^A)

            var message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "A",
                                    clientStamp: DateTimeOffset.UtcNow,
                                    serviceStamp: DateTimeOffset.UtcNow,
                                    operationKind: OperationKind.Start);
            var json = JsonConvert.SerializeObject(message);
            var cloudMessage = new CloudQueueMessage(json);
            await queue.AddMessageAsync(cloudMessage);

            #endregion // #L1(^A)

            await Task.Delay(DELAY);

            #region L1(^A)-|

            message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "A",
                                    clientStamp: DateTimeOffset.UtcNow,
                                    serviceStamp: DateTimeOffset.UtcNow,
                                    operationKind: OperationKind.Stop);
            json = JsonConvert.SerializeObject(message);
            cloudMessage = new CloudQueueMessage(json);
            await queue.AddMessageAsync(cloudMessage);

            #endregion // L1(^A)-|
        }

        #endregion // SimulateStartStopAsync

        #region SimulateStartAndDuplicateTabAsync

        //  #L1(^A)----------------|
        //          #L1(^B)-----|
        private static async Task SimulateStartAndDuplicateTabAsync(CloudQueue queue)
        {
            #region #L1(^A)

            var message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "A",
                                    clientStamp: DateTimeOffset.UtcNow,
                                    serviceStamp: DateTimeOffset.UtcNow,
                                    operationKind: OperationKind.Start);
            var json = JsonConvert.SerializeObject(message);
            var cloudMessage = new CloudQueueMessage(json);
            await queue.AddMessageAsync(cloudMessage);

            #endregion // #L1(^A)

            await Task.Delay(DELAY);

            #region     #L1(^B)

            message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "B",
                                    clientStamp: DateTimeOffset.UtcNow,
                                    serviceStamp: DateTimeOffset.UtcNow,
                                    operationKind: OperationKind.Start);
            json = JsonConvert.SerializeObject(message);
            cloudMessage = new CloudQueueMessage(json);
            await queue.AddMessageAsync(cloudMessage);

            #endregion //   #L1(^B)

            await Task.Delay(DELAY);

            #region     L1(^B)----|

            message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "B",
                                    clientStamp: DateTimeOffset.UtcNow,
                                    serviceStamp: DateTimeOffset.UtcNow,
                                    operationKind: OperationKind.Stop);
            json = JsonConvert.SerializeObject(message);
            cloudMessage = new CloudQueueMessage(json);
            await queue.AddMessageAsync(cloudMessage);

            #endregion //      L1(^B)----|

            await Task.Delay(DELAY);

            #region L1(^A)----------------|

            message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "A",
                                    clientStamp: DateTimeOffset.UtcNow,
                                    serviceStamp: DateTimeOffset.UtcNow,
                                    operationKind: OperationKind.Stop);
            json = JsonConvert.SerializeObject(message);
            cloudMessage = new CloudQueueMessage(json);
            await queue.AddMessageAsync(cloudMessage);

            #endregion // L1(^A)----------------|
        }

        #endregion // SimulateStartAndDuplicateTabAsync
    }
}
