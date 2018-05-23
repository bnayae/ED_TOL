using System;

namespace LessonContracts
{
    // sent from the client via the queue
    public interface ILessonQueueMessage
    {
        object Token { get; }
        // number of time this message was dequeue
        int RetryCount { get;  }
        LessonMessage Data { get; set; }
    }
}