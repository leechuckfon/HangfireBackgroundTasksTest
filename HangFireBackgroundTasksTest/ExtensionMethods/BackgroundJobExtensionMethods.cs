using Hangfire;
using HangFireBackgroundTasksTest.OneOffJobs;
using System;

namespace HangFireBackgroundTasksTest {
    public static class BackgroundJobExtensionMethods {
       
        public static void QueueBackgroundJobs(this IBackgroundJobClient backgroundJobs) {

            backgroundJobs.Enqueue(() => Job2.Run());
            backgroundJobs.Enqueue(() => AsyncJob3.Run());
            backgroundJobs.Schedule(() => SingleExecutionFail.Run(), TimeSpan.FromMinutes(1));
        }
    }
}
