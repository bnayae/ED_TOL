using System;

namespace LessonContracts
{
    // sent from the client via the queue
    public class LessonMessage
    {
        [Obsolete("Only for deserialize", true)]
        public LessonMessage() { }
        public LessonMessage(
            Guid loginSessionId,
            int userId,
            int lessonId,
            int lessonSessionId,
            int stepId,
            string tabId,
            DateTimeOffset clientStamp,
            DateTimeOffset serviceStamp,
            OperationKind operationKind)
        {
            LoginSessionId = loginSessionId;
            UserId = userId;
            LessonId = lessonId;
            LessonSessionId = lessonSessionId;
            StepId = stepId;
            TabId = tabId;
            ClientStamp = clientStamp;
            ServiceStamp = serviceStamp;
            OperationKind = operationKind;
        }
        public Guid LoginSessionId { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public int LessonSessionId { get; set; }
        public int StepId { get; set; }
        public string TabId { get; set; }
        public DateTimeOffset ClientStamp { get; set; }
        public DateTimeOffset ServiceStamp { get; set; }
        public OperationKind OperationKind { get; set; }
    }
}