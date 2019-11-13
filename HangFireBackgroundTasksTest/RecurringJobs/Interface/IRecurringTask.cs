namespace HangFireBackgroundTasksTest {
    // Interfaces not used for cleaner code during job assignment

    internal interface IRecurringTask {
        string CronExpression { get; }

        void Run();
        void Init();
    }
}