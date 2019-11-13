using Hangfire;
using System;
using System.IO;

namespace HangFireBackgroundTasksTest.OneOffJobs {
    // A job that will fail on purpose

    public class SingleExecutionFail {
        [AutomaticRetry(Attempts = 1, DelaysInSeconds = new[] { 50 }, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public static void Run() {
            try {
                throw new IOException("Single execution Failed");
            }
            catch (Exception e) {
                throw e;
            }
        }
    }
}
