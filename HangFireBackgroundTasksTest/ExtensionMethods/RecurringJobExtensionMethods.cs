using Hangfire;
using HangFireBackgroundTasksTest.RecurringJobs;

namespace HangFireBackgroundTasksTest {
    public static class RecurringJobExtensionMethods {
        public static void QueueRecurringJobs(this IRecurringJobManager manager) {
            manager.AddOrUpdate(nameof(Job1), () => Job1.Run(), Job1.CronExpression);
            manager.AddOrUpdate(nameof(AsyncJob4), () => AsyncJob4.Run(), AsyncJob4.CronExpression);
            manager.AddOrUpdate(nameof(FailingRecurring), () => FailingRecurring.Run(), FailingRecurring.CronExpression);
            manager.AddOrUpdate<RecurringIOCTest>(nameof(RecurringIOCTest), x => x.Run(), Cron.Minutely());
        }
    }
}
