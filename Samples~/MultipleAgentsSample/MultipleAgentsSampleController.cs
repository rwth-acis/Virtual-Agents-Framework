using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class MultipleAgentsSampleController : SampleScheduleController
    {
        /// <summary>
        /// List of waypoints which the agents should visit.
        /// </summary>
        [Tooltip("List of waypoints which the agents should visit.")]
        public List<Transform> waypoints;
        [Tooltip("Secondary agent that goes to separate waypoint.")]
        public Agent agentTwo;
        // The other agent needs to use their own TaskSystem
        private ScheduleBasedTaskSystem taskSystem2;
        protected override void Start()
        {
            if (waypoints == null || waypoints.Count < 2 || waypoints[0] == null || waypoints[1] == null)
            {
                Debug.LogError("MultipleAgentsSampleController requires at least 2 waypoints.");
                return;
            }
            base.Start();
            taskSystem.Tasks.GoTo(waypoints[0]);
            taskSystem2 = (ScheduleBasedTaskSystem)agentTwo.TaskSystem;
            taskSystem2.Tasks.GoTo(waypoints[1]);
        }
    }
}