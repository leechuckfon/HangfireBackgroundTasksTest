namespace HangFireBackgroundTasksTest {
    // Interfaces not used for cleaner code during job assignment
    internal interface ISingleExecutionTask {
        void Run();
        void Init();
    }
}