using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// A task that directly manipualtes an agent
    /// </summary>
    public interface IAgentTask : ITask
    {
        /// <summary>
        /// Indicates whether the task can start its execution
        /// False if there are unfulfilled conditions that block the execution
        /// </summary>
        bool CanStart { get; }

        /// <summary>
        /// Aborts the task
        /// </summary>
        public void Abort() {}

        event Action OnTaskFinished;
        event Action OnTaskStarted;
    }
}