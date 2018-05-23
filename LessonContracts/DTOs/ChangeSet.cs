using System;

namespace LessonContracts
{
    public class ChangeSet
    {
        public ChangeSet(Lesson added, params Lesson[] modified)
        {
            Added = added;
            Modified = modified;
        }

        public Lesson Added { get; }
        public Lesson[] Modified { get; }
    }

}