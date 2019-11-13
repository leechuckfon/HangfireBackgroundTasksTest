using Hangfire.Logging;
using System;

namespace HangFireBackgroundTasksTest {
    public static class Job1 {
        public static string CronExpression { get => "* * * ? * *"; }

        public static void Init() {
            Console.WriteLine("The recurring task has been initialized");
        }

        public static void Run() {
            Console.WriteLine("Recurring Job has been executed");

        }
    }
}
