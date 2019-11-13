using Hangfire;
using HangFireBackgroundTasksTest.IoC;
using HangFireBackgroundTasksTest.IoC.Interface;
using HangFireBackgroundTasksTest.Logging;
using Microsoft.AspNetCore.Builder;
using System;
using Unity;

namespace HangFireBackgroundTasksTest {
    public class Startup {
        public void Configure(IApplicationBuilder appBuilder, IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobs) {
            appBuilder.UseHangfireDashboard("");
            // Has to be called before adding jobs
            GlobalConfiguration.Configuration.UseUnityActivator(setupContainer());
            appBuilder.UseRouting();
            appBuilder.UseHangfireServer();
            GlobalConfiguration.Configuration.UseLogProvider(new CustomLogProvider());
            //Use extension methods to add jobs
            backgroundJobs.QueueBackgroundJobs();
            recurringJobs.QueueRecurringJobs();

        }

        private static IUnityContainer setupContainer() {
            //Register Unity via the Hangfire.Unity package (2 years outdated?)
            var _container = new UnityContainer();
            _container.RegisterType<IIOCTest,IOCTest>();
            return _container as IUnityContainer;
        }
    }
}
