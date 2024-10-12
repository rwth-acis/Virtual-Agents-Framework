using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public abstract class TaskSystem : MonoBehaviour, ITaskSystem
    {
        /// <summary>
        /// Updates the task system
        /// </summary>
        public abstract void UpdateTaskSystem();
    }
}
