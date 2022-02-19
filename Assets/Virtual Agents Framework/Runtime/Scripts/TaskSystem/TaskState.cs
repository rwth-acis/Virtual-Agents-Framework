namespace VirtualAgentsFramework.TaskSystem
{
    /// <summary>
    /// States an agent's tasks can be in
    /// </summary>
    public enum TaskState
    {
        inactive, // i.e. requesting new tasks is disabled
        idle, // i.e. requesting new tasks is enabled
        busy // i.e. currently executing a task
    }
}
