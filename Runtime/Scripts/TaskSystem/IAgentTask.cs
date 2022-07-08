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
    }
}