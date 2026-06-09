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
            base.Start();
            taskSystem.Tasks.GoTo(waypoints[0]);
            this.agent = agentTwo;
            taskSystem2 = (ScheduleBasedTaskSystem)agent.TaskSystem;
            taskSystem2.Tasks.GoTo(waypoints[1]);
        }
    }
}