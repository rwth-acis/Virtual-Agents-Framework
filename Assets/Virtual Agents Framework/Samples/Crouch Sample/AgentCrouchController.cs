using System.Collections;
using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentCrouchController : SampleScheduleController
    {
        /// <summary>
        /// List of waypoints which the agent should visit in order.
        /// </summary>
        [Tooltip("List of waypoints which the agent should visit in order.")]
        public List<Transform> waypoints;

        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < waypoints.Count-1; i++)
            {
                taskSystem.Tasks.GoTo(waypoints[i].position, 0, 1f);
            }
        }

    }
}