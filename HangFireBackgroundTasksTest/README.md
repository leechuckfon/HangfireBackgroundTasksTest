# Hangfire Summary

## Hangfire Installation

Installing by using:
ASP.NET: `Install-Package Hangfire`
Windows Services/Console Apps: `Install-Package Hangfire.Core`

Console App Example:

    static void Main(string[] args) {
        // Use In Memory Database
        JobStorage.Current = GlobalConfiguration.Configuration.UseMemoryStorage();
        // Implicit Start (start of using) and Stop (when disposed) of server
        using (var server = new BackgroundJobServer()) {
            //Adding a recurring job to the server
            RecurringJob.AddOrUpdate(() => doSomething(), Cron.Minutely);
            Console.ReadKey();
        }
    }
    // Jobs can only call public methods
    public static void doSomething() {
        privateDoSomething();
    }
    // But they can in turn call private ones
    private static void privateDoSomething() {
        Console.WriteLine("LOL");
    }

## Database

Hangfire can make use of a database to store data regarding running jobs. The data that hangfire can access can also be limited by creating a user in the SQL Server specifically for the Hangfire instance like so:

```
CREATE USER [HangFire] WITH PASSWORD = 'strong_password_for_hangfire'
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = 'HangFire') EXEC ('CREATE SCHEMA [HangFire]')
GO

ALTER AUTHORIZATION ON SCHEMA::[HangFire] TO [HangFire]
GO

GRANT CREATE TABLE TO [HangFire]
GO
```

The database tables can also be used to add more jobs while the server is still running it's current jobs. For this there is a way to poll the SQL Server like so:

```
var options = new SqlServerStorageOptions
{
    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
    QueuePollInterval = TimeSpan.Zero
};

GlobalConfiguration.Configuration.UseSqlServerStorage("<name or connection string>", options);
```

Note that this will halt the invocation of the background jobs until the polling is done.

## Dashboard
 ### ASP NET
Adding a dashboard is possile to using an OwinStartup file where you use the extension method `.UseHangfireDashboard()` to set up the dashboard.

The dashboard can be secured to specific users by writing an AuthorizationFilter like so:
```
public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In case you need an OWIN context, use the next line, `OwinContext` class
        // is the part of the `Microsoft.Owin` package.
        var owinContext = new OwinContext(context.GetOwinEnvironment());

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        return owinContext.Authentication.User.Identity.IsAuthenticated;
    }
}
```

Assign it like so:
````
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new [] { new MyAuthorizationFilter() }
});
````

In `Configuration` method, call Authentication first an UseHangFireDashboard last to make it effective.

You can give a string to the use dashboard method to map the dashboard page to a different endpoint.

### Console App

Main of the Console App will build webhost, load in a config file, add MVC and add Hangfire.
```
static void Main(string[] args) {
            // Getting configuration from appsettings.json and building it
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot root = builder.Build();

            // Making a webhost so that the dashboard is hosted
            var host = new WebHostBuilder()
                        .UseConfiguration(root)
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseEnvironment("Development")
                        .UseIISIntegration()
                        .ConfigureServices(services => {
                            services.AddHangfire(configuration => {
                                configuration
                                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                .UseSimpleAssemblyNameTypeSerializer()
                                .UseRecommendedSerializerSettings()
                                .UseMemoryStorage();
                            });
                            services.AddMvc();
                            services.AddHangfireServer();
                        })
                        .UseKestrel()
                        .UseStartup<Startup>()
                        .Build();

            host.Run();

        }
```

Startup will configure the Hangfire dashboard and configure it to a route (here https://localhost:5000), use Routing, optionally add a unity container and add the Hangfire server for startup. (Here we also call `backgroundJobs.QueueBackgroundJobs();` and `recurringJobs.QueueRecurringJobs();` to register different jobs to the client for single execution jobs and manager for recurring jobs respectively). We also call `setupContainer()` to configure the UnityContainer used for DI.

```
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
```

### Adding Tasks

You can add tasks by adding them to either the RecurringJobManager (for recurring jobs) or the BackgroundJobClient (for single execution jobs) like so:

```
// Without delay
backgroundJobs.Enqueue(() => AsyncJob3.Run());
// With delay
backgroundJobs.Schedule(() => SingleExecutionFail.Run(), TimeSpan.FromMinutes(1));
```

```
// Without the need of DI
manager.AddOrUpdate(nameof(FailingRecurring), () => FailingRecurring.Run(), FailingRecurring.CronExpression);
// With or Without the need of DI, x will use the class mentioned in the arrow brackers (<>)
manager.AddOrUpdate<RecurringIOCTest>(nameof(RecurringIOCTest), x => x.Run(), Cron.Minutely());
```

### Scheduled Tasks

Scheduled tasks are tasks with a delay (single execution) or tasks which have failed and need to be requeued for retry (singl execution or recurring tasks).

### Failing Tasks

When a task fails the server will infinitely retry the task until it finally succeeds. This can also be configured manually like this:

`[AutomaticRetry(Attempts = 1, DelaysInSeconds = new[] { 50 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]`

In above example the attribute (attached to the method that is ran by the BackgroundJobs/RecurringJobsManager object) the method will retry once when it fails with a delay of 50 seconds. Once it has failed twice (once on initial execution and once as a retry) it will put the Tasks in the 'FAILED' category of the dashboard.

The following example will do the same but put it in the 'DELETED' category instead:

`[AutomaticRetry(Attempts = 1, DelaysInSeconds = new[] { 50 }, OnAttemptsExceeded = AttemptsExceededAction.Delete)]`

!!! Exceptions thrown with messages will not be displayed in the logs !!!

### DELETED vs FAILED

The biggest difference between putting a task in the DELETED category or the FAILED category is that when it is put in the DELETED category, the failed task will be automatically deleted from the log in 1 day. When put in FAILED you need to delete or requeue the log mmanually.

In both scenario's you will see a list of failed/deleted Tasks and when clicked you can see the life cycle of the selected Task including:

- When the Task was scheduled
- How it was Enqueued
- Which server and which worker processed the Task (since you can run multiple    servers attached to different SQL Servers)
- What exception was thrown and where it occured
- When the log in the FAILED/DELETED tab was created
- The time every step took
