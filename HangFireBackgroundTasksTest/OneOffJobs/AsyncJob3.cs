using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HangFireBackgroundTasksTest.OneOffJobs {
    // A job that will try to do something asynchronously

    public class AsyncJob3 {

        public static async Task Init() {
            Console.WriteLine("The recurring task has been initialized");
        }

        public static async Task Run() {
            using (var client = new HttpClient()) {
                var response = await client.GetAsync("http://www.isitfridayyet.org");
                Console.WriteLine("IIFY? (single execution) call: " + response.StatusCode);
            }
        }
    }
}
