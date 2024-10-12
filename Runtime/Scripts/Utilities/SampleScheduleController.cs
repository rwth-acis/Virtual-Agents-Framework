using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.ScheduleBasedExecution;

namespace i5.VirtualAgents.Examples
{
    /// <summary>
    /// This script is for the varios sample scripts that use the ScheduleBasedTaskSystem. It provides a reference to the agent and the task system. The agent should be set in the inspector.
    /// It is not recommended to use this script in your own projects. Instead, implement your own script that inherits from MonoBehaviour and uses an agent and task system references that is set there.
    /// </summary>
    public class SampleScheduleController : MonoBehaviour
    {
        /// <summary>
        /// The agent which will execute the tasks.
        /// </summary>
        public Agent agent;
        protected ScheduleBasedTaskSystem taskSystem;

        protected virtual void Start()
        {
            if (agent == null)
            {
                Debug.LogError("No agent set in the inspector. Please set an agent or implement your own version of the SampleScheduleController to set agent in code.");
            }
            taskSystem = (ScheduleBasedTaskSystem)agent.TaskSystem;
        }
    }
}
