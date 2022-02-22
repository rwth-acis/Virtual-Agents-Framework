using System;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    /// <summary>
    /// Base class which provides default implementations for the interface methods
    /// Using this class avoids repeatedly implementing empty interface classes if they are not needed
    /// </summary>
    public abstract class AgentBaseTask : IAgentTask
    {
        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        public virtual void Execute(Agent agent) { }

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Event which is invoked once the task is finished
        /// Subscribed to by the agent's task manager so that the next task can be started afterwards
        /// </summary>
        public event Action OnTaskFinished;

        /// <summary>
        /// Marks the task as finished for the executing agent
        /// This will raise the OnTaskFinished event in the base class
        /// Use this method to finish the task
        /// as invoking the event in derived classes is not possible (https://stackoverflow.com/a/31661451)
        /// </summary>
        protected virtual void MarkAsFinished()
        {
            OnTaskFinished();
        }
    }
}