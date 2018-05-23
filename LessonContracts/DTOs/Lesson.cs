using System;
using System.ComponentModel.DataAnnotations;

namespace LessonContracts
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        public Guid LoginSessionId { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public int LessonSessionId { get; set; }
        public int StepId { get; set; }
        public string TabId { get; set; }
        public DateTimeOffset StartClientStamp { get; set; }
        public DateTimeOffset StartServiceStamp { get; set; }
        public ActionReason StartReason { get; set; }

        // always service stamping 
        public DateTimeOffset? EndServiceStamp { get; set; }
        public ActionReason EndReason { get; set; }   
        
        // when the system close the lesson from other reasons 
        // we still keeping the final close (by user or timeout)
        // this information shouldn't be part of the report calculation
        public DateTimeOffset? FinalEndClientStamp { get; set; } 
        public DateTimeOffset? FinalEndServiceStamp { get; set; } 
    }
}