namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// The different states that the agent's task manager can be in
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// The task manager is inactive, e.g. if no agent is associated with it yet or if it has been deactived deliberately
        /// In an inactive state, no tasks are executed or started
        /// </summary>
        inactive,
        /// <summary>
        /// An idle state where nothing is to do and new tasks can be started
        /// </summary>
        idle,
        /// <summary>
        /// The agent is busy and is currently executing a task
        /// </summary>
        busy
    }
}
