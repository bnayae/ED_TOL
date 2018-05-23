using System;
using LessonContracts;

namespace LessonAzureQueueProvider
{
    // sent from the client via the queue
    public class LessonQueueMessage: ILessonQueueMessage
    {
        public LessonQueueMessage(
            object token,
            int retryCount,
            LessonMessage data)
        {
            Token = token;
            RetryCount = retryCount;
            Data = data;
        }

        public object Token { get; }
        // number of time this message was dequeue
        public int RetryCount { get;  }
        public LessonMessage Data { get; set; }

    }
}