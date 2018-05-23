using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonContracts
{
    public interface ILessonJob
    {
        Task ProcessAsync(LessonMessage data); 
    }
}
