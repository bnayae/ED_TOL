using System;
using LessonContracts;
using LessonJob;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;
using System.Threading.Tasks;

/*
* +-------------------------------------------------+
* |   ^        = tab                                |
* |   L        = Lesson session identifier          |
* |   #        = Start                              |
* |   -|       = End                                |
* |   #L1(^A)-| = Start Lesson 1 on Tab A, end it   |
* +-------------------------------------------------+
*/

namespace LessonUnitTests
{
    [TestClass]
    public class LessonJobTests
    {
        private LessonProcessor _processor;
        private ILessonRepository _repository = A.Fake<ILessonRepository>();
        private static readonly Guid LOGIN_SESSION_ID = Guid.NewGuid();
        private readonly DateTimeOffset _baseTime = DateTimeOffset.Now;

        [TestInitialize]
        public void Setup()
        {
            _processor = new LessonProcessor(_repository);
        }

        [TestMethod]
        public async Task Lesson_Start_Test()
        {
            A.CallTo(() => _repository.GetActiveLessonAsync(A<Guid>.Ignored, A<int>.Ignored))
                .Returns(Task.FromResult(Array.Empty<Lesson>()));

            #region #L1(^A)

            var message = new LessonMessage(
                                    loginSessionId: LOGIN_SESSION_ID,
                                    userId: 1,
                                    lessonId: 10,
                                    lessonSessionId: 101,
                                    stepId: 20,
                                    tabId: "A",
                                    clientStamp: _baseTime,
                                    serviceStamp: _baseTime.AddSeconds(1),
                                    operationKind: OperationKind.Start);

            #endregion // #L1(^A)

            await _processor.ProcessAsync(message);
            A.CallTo(() => _repository.UpsertLessonAsync(
                                        A<ChangeSet>.That.Matches(
                                            c => c.Added.StartReason == ActionReason.Client &&
                                                 c.Added.StartServiceStamp == message.ServiceStamp &&
                                                 c.Added.StartClientStamp == message.ClientStamp &&
                                                 c.Added.TabId == message.TabId &&
                                                 c.Added.LessonSessionId == message.LessonSessionId &&
                                                 c.Added.UserId == message.UserId &&
                                                 c.Modified.Length == 0)))
                                        .MustHaveHappenedOnceExactly();
        }
    }
}
