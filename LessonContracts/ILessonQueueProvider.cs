using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonContracts
{
    public interface ILessonQueueProvider
    {
        Task<ILessonQueueMessage> DequeueAsync();
        Task DeleteAsync(object token);
        Task CancelAsync(object token);
    }
}
