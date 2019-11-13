using HangFireBackgroundTasksTest.IoC.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangFireBackgroundTasksTest.IoC {
    public class IOCTest : IIOCTest {
        public void TestIOC() {
            Console.WriteLine("I just got injected and got called");
        }
    }
}
