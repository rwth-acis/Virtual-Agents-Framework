using i5.VirtualAgents.AgentTasks;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AdaptiveGazeSampleController : SampleScheduleController
    {
        [SerializeField] private List<GameObject> waypoints;

        [SerializeField] private GameObject target;
        [SerializeField] private bool overwriteAdaptiveGazeWithAimHead = false;
        [SerializeField] private int aimAtTime = 100;

        protected override void Start()
        {
            base.Start();
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut but we could also just create a new
            // AgentMovementTask and schedule it using agent.ScheduleTask.
            for (int i = 0; i < waypoints.Count; i++)
            {
                taskSystem.Tasks.GoTo(waypoints[i], default);
            }
            if (overwriteAdaptiveGazeWithAimHead)
            {
                AgentBaseTask pointingHead = taskSystem.Tasks.PlayAnimation("NoAnimation", aimAtTime, "", 0, "Head", target);
            }
        }
    }
}