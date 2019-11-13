using HangFireBackgroundTasksTest.IoC.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangFireBackgroundTasksTest.RecurringJobs {
    public class RecurringIOCTest {
        public string CronExpression { get; set; }
        public IIOCTest testService { get; set; }
        public RecurringIOCTest(IIOCTest test) {
            testService = test;
        }

        public void Run() {
            testService.TestIOC();
        }
    }
}
