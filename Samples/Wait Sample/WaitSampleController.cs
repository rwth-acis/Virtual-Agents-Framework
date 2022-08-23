using i5.VirtualAgents;
using UnityEngine;
using i5.VirtualAgents.ScheduleBasedExecution;

namespace i5.VirtualAgents.Examples
{
    public class WaitSampleController : SampleScheduleController
    {
        public Transform[] waypoints;

        public override void Start()
        {
            base.Start();
            taskSystem.Tasks.GoTo(waypoints[0]);
            taskSystem.Tasks.WaitForSeconds(2f);
            taskSystem.Tasks.GoTo(waypoints[1]);
        }
    }
}