using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentNavigationController : SampleScheduleController
    {
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;

        protected override void Start()
        {
            base.Start();
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut but we could also just create a new
            // AgentMovementTask and schedule it using agent.ScheduleTask.
            for (int i = 0; i < waypoints.Count; i++)
            {
                taskSystem.Tasks.GoTo(waypoints[i].position);
            }

            // example for a different priority:
            // this waypoint is added last but has the highest priority,
            // so the agent will walk to it first
            taskSystem.Tasks.GoTo(highPrioWaypoint, Vector3.zero, 5);
        }
    }
}