using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonAzureQueueProvider;
using LessonContracts;
using LessonDAL;
using LessonJob;

namespace LessonConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ILessonRepository repository = new LessonRepository();
            ILessonQueueProvider queueProvider = new QueueProvider();
            ILessonJob job = new LessonProcessor(repository);

            Task t = ProcessAsync(queueProvider, job);
            t.Wait();
        }

        private static async Task ProcessAsync(
                                    ILessonQueueProvider queueProvider,
                                    ILessonJob job)
        {
            while (true)
            {
                ILessonQueueMessage message = null;
                try
                {
                    message = await queueProvider.DequeueAsync();
                    Console.WriteLine($"Processing L{message.Data.LessonSessionId}(^{message.Data.TabId}) {message.Data.OperationKind}");
                    await job.ProcessAsync(message.Data);
                    await queueProvider.DeleteAsync(message.Token);
                }
                catch (Exception ex)
                {
                    await queueProvider.CancelAsync(message?.Token);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\n{ex.GetBaseException().Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
