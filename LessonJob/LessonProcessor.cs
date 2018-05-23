using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonContracts;

namespace LessonJob
{
    public class LessonProcessor : ILessonJob
    {
        private readonly ILessonRepository _repository;

        #region Ctor

        public LessonProcessor(ILessonRepository repository)
        {
            _repository = repository;
        }

        #endregion // Ctor

        #region HandleStartAsync

        public async Task ProcessAsync(LessonMessage data)
        {
            Lesson[] openedLessens =
                await _repository.GetActiveLessonAsync(
                                        data.LoginSessionId,
                                        data.UserId);
            ChangeSet changeSet = null;
            if (data.OperationKind == OperationKind.Start)
                changeSet = await HandleStartAsync(data, openedLessens);
            else
                changeSet = await HandleEndAsync(data, openedLessens);

            await _repository.UpsertLessonAsync(changeSet);
        }

        #endregion // HandleStartAsync

        #region HandleStartAsync

        public async Task<ChangeSet> HandleStartAsync(
                            LessonMessage data,
                            Lesson[] openedLessens)
        {
            Lesson added = null;
            Lesson duplicateStat = openedLessens.FirstOrDefault(l =>  // avoid duplicate processing
                        l.LessonSessionId == data.LessonSessionId &&
                        l.TabId == data.TabId &&
                        l.EndReason == ActionReason.Undefined);
            if (duplicateStat == null)
            {
                added = new Lesson
                {
                    TabId = data.TabId,
                    UserId = data.UserId,
                    LoginSessionId = data.LoginSessionId,
                    LessonId = data.LessonId,
                    LessonSessionId = data.LessonSessionId,
                    StepId = data.StepId,
                    StartClientStamp = data.ClientStamp,
                    StartServiceStamp = data.ServiceStamp,
                    StartReason = ActionReason.Client,
                };
            }
            else
                Trace.WriteLine($"Duplicate processing of start [{duplicateStat.StartServiceStamp}] and [{data.ServiceStamp}]");

            Lesson[] changes = openedLessens
                                .Where(l => l.EndReason == ActionReason.Undefined)
                                .ToArray();
            foreach (var lesson in changes)
            {
                lesson.EndReason = ActionReason.Deactivate;
                lesson.EndServiceStamp = data.ServiceStamp;
            }

            return new ChangeSet(added, changes);
        }

        #endregion // HandleStartAsync

        #region HandleEndAsync

        public async Task<ChangeSet> HandleEndAsync(
                            LessonMessage data,
                            Lesson[] openedLessens)
        {
            Lesson[] sameLessons = openedLessens
                        .Where(l => l.LessonSessionId == data.LessonSessionId &&
                                    l.TabId == data.TabId)
                        .ToArray();
            foreach (var lesson in sameLessons)
            {
                if (lesson.EndReason == ActionReason.Undefined)
                {
                    lesson.EndReason = ActionReason.Client;
                    lesson.EndServiceStamp = data.ServiceStamp;
                }
                if (!lesson.FinalEndClientStamp.HasValue)
                    lesson.FinalEndClientStamp = data.ClientStamp;
                if (!lesson.FinalEndServiceStamp.HasValue)
                    lesson.FinalEndServiceStamp = data.ServiceStamp;
            }
            Lesson[] otherLessons = openedLessens
                        .Where(l => (l.LessonSessionId != data.LessonSessionId ||
                                        l.TabId != data.TabId) &&
                                     l.EndReason == ActionReason.Undefined)
                        .ToArray();
            foreach (var lesson in otherLessons)
            {
                lesson.EndReason = ActionReason.Deactivate;
                lesson.EndServiceStamp = data.ServiceStamp;
            }

            Lesson reopen = openedLessens
                                .OrderByDescending(l => l.StartServiceStamp)
                                .FirstOrDefault(l =>
                                    l.LessonSessionId != data.LessonSessionId);
            Lesson added = null;
            if (reopen != null)
            {
                added = new Lesson
                {
                    TabId = reopen.TabId,
                    UserId = reopen.UserId,
                    LoginSessionId = reopen.LoginSessionId,
                    LessonId = reopen.LessonId,
                    LessonSessionId = reopen.LessonSessionId,
                    StepId = reopen.StepId,
                    StartClientStamp = data.ClientStamp,
                    StartServiceStamp = data.ServiceStamp,
                    StartReason = ActionReason.Deactivate,
                };
            }
            Lesson[] changes = sameLessons.Concat(otherLessons).ToArray();
            return new ChangeSet(added, changes);
        }

        #endregion // HandleEndAsync
    }
}
