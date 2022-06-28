using System;
using System.Collections.Generic;

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
        public virtual void Execute(Agent agent) {}

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        public virtual TaskState Update()
        {
            return rootState;
        }



        public virtual void Stop(){}

        public virtual void FinishTask()
        {
            rootState = TaskState.Success;
        }

        public List<Func<bool>> ReadyToStart { get; set; }

        public List<Func<bool>> ReadyToEnd { get; set; }
        public TaskState rootState { get; set; }
    }
}