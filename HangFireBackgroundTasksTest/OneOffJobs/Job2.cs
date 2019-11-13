using System;

namespace HangFireBackgroundTasksTest.OneOffJobs {
    public static class Job2 {

        public static void Init() {
            Console.WriteLine("Job 2 has been initialized");
        }

        public static void Run() {
            Console.WriteLine("Job 2 has been executed");
        }
    }
}
