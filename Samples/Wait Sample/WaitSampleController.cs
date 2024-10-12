using i5.VirtualAgents;
using UnityEngine;
using i5.VirtualAgents.ScheduleBasedExecution;

namespace i5.VirtualAgents.Examples
{
    public class WaitSampleController : SampleScheduleController
    {
        /// <summary>
        /// The waypoints which the agent should visit in order.
        /// </summary>
        [Tooltip("The waypoints which the agent should visit in order.")]
        public Transform[] waypoints;

        protected override void Start()
        {
            base.Start();
            taskSystem.Tasks.GoTo(waypoints[0]);
            taskSystem.Tasks.WaitForSeconds(2f);
            taskSystem.Tasks.GoTo(waypoints[1]);
        }
    }
}