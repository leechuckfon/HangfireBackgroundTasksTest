using Hangfire;
using System;
using System.IO;

namespace HangFireBackgroundTasksTest.RecurringJobs {
    // A job that will fail on purpose
    public class FailingRecurring {
        public static string CronExpression { get => Cron.Minutely(); }
        [AutomaticRetry(Attempts = 1, DelaysInSeconds = new[] { 50 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public static void Run() {
            try {
                throw new IOException();
            }
            catch (Exception e) {
                throw e;
            }
        }

    }
}
