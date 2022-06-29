using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Common methods and attributes for all AgentTasks
    /// </summary>
    public interface IAgentTask : ITask
    {
        bool CanStart { get; }

        /// <summary>
        /// Event which is invoked once the task is finished
        /// Subscribed to by the agent's task manager so that the next task can be started afterwards
        /// </summary>
        event Action OnTaskFinished;
    }
}