using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class ItemPickUpSampleController: SampleScheduleController
    {
        public List<GameObject> waypoints;

        protected override void Start()
        {
            base.Start();
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut but we could also just create a new
            // AgentMovementTask and schedule it using agent.ScheduleTask.
            for (int i = 0; i < waypoints.Count; i++)
            {
                taskSystem.Tasks.GoToAndPickUp(waypoints[i],default);
            }
        }
    }
}