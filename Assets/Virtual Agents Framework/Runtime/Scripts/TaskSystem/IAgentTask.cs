using System;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Common methods and attributes for all AgentTasks
    /// </summary>
    public interface IAgentTask
    {
        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        void Execute(Agent agent);

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        void Update();

        Func<bool> PrepareSchedule { get; set; }

        /// <summary>
        /// Event which is invoked once the task is finished
        /// Subscribed to by the agent's task manager so that the next task can be started afterwards
        /// </summary>
        event Action OnTaskFinished;
    }
}