using Hangfire;
using HangFireBackgroundTasksTest.OneOffJobs;
using System;

namespace HangFireBackgroundTasksTest {
    public static class BackgroundJobExtensionMethods {
        public static string ConfigString  = @"{'DestinationPath' : '\\10.3.32.127\Invoices\Medius',
                'MediusBaseUrl' : 'https://cloud.mediusflow.com/kinepolisQA',
                'Scope' : 'Integration',
                'GrantType' : 'client_credentials',
                'Client_id' : '42aa24eb61d94114412b1d6328e961fa',
                'Client_secret' : 'Ng-gVgD2nDULBjKIW4NbarxtkJcteQMjCzXy4VfCoi8'}";
        public static void QueueBackgroundJobs(this IBackgroundJobClient backgroundJobs) {

            backgroundJobs.Enqueue(() => Job2.Run());
            backgroundJobs.Enqueue(() => AsyncJob3.Run());
            backgroundJobs.Schedule(() => SingleExecutionFail.Run(), TimeSpan.FromMinutes(1));
        }
    }
}
