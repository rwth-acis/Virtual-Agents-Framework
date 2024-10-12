using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Provides means for updating a task system and scheduling new tasks.
    /// </summary>
    public interface ITaskSystem
    {
        /// <summary>
        /// Updates the task system
        /// </summary>
        void UpdateTaskSystem();
    }
}
