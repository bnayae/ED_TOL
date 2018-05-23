using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonContracts;

namespace LessonDAL
{
    internal class LessonContext : DbContext
    {
        public LessonContext():
            base("Edusoft.LessonDB")
        {
        }

        public DbSet<Lesson> Lessons { get; set; }
    }
}
