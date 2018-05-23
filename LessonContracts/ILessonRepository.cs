using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonContracts
{
    public interface ILessonRepository
    {
        Task<Lesson[]> GetActiveLessonAsync(Guid loginSessionId, int userId); 
        Task UpsertLessonAsync(ChangeSet changes); 
    }
}
