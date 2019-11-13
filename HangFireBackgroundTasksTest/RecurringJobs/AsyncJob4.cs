using Hangfire;
using System;
using System.Net.Http;
using System.Threading.Tasks;
namespace HangFireBackgroundTasksTest.RecurringJobs {
// A job that will try to do something asynchronously
    public static class AsyncJob4 {

        public static string CronExpression { get => Cron.Minutely(); }

        public static async Task Init() {
            Console.WriteLine("The recurring task has been initialized");
        }

        public static async Task Run() {
            using (var client = new HttpClient()) {
                var response = await client.GetAsync("http://www.isitfridayyet.org");
                Console.WriteLine("IIFY (recurring) call: " + response.StatusCode);
            }
        }
    }
}
