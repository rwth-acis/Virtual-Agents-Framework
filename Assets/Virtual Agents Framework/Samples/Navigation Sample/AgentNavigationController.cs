using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentNavigationController : MonoBehaviour
    {
        public Agent agent;
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;

        void Start()
        {
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut but we could also just create a new
            // AgentMovementTask and schedule it using agent.ScheduleTask.
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.GoTo(waypoints[i].position);
            }

            // example for a different priority:
            // this waypoint is added last but has the highest priority,
            // so the agent will walk to it first
            agent.Tasks.GoTo(highPrioWaypoint, Vector3.zero, 5);
        }
    }
}