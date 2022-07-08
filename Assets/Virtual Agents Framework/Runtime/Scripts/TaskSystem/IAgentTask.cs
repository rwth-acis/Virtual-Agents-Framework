using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// A task that directly manipualtes an agent
    /// </summary>
    public interface IAgentTask : ITask
    {
        bool CanStart { get; }
    }
}