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

            // add a task to go to the start an drop the first item
            taskSystem.Tasks.GoToAndDropItem(Vector3.zero,waypoints[0]);

            // add a task to move a bit and then drop all item
            taskSystem.Tasks.GoToAndDropItem(new Vector3(10,0,0));

            //move away from the items
            taskSystem.Tasks.GoTo(new Vector3(5,0,0));
        }
    }
}