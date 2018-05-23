using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonContracts;

namespace LessonDAL
{
    public class LessonRepository : ILessonRepository
    {
        public async Task<Lesson[]> GetActiveLessonAsync(
            Guid loginSessionId,
            int userId)
        {
            using (var context = new LessonContext())
            {
                var query = from item in context.Lessons
                            where item.LoginSessionId == loginSessionId &&
                                  item.FinalEndClientStamp == null
                            select item;
                var results = await query.ToArrayAsync();
                return results;
            }
        }

        public async Task UpsertLessonAsync(ChangeSet changes)
        {
            Lesson added = changes.Added;
            Lesson[] modifies = changes.Modified;
            using (var context = new LessonContext())
            {
                if (added != null)
                    context.Lessons.Add(added);
                foreach (Lesson modified in modifies)
                {
                    context.Lessons.Attach(modified);
                    context.Entry(modified).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
