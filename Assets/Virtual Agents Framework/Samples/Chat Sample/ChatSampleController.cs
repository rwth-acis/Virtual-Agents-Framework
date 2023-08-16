using i5.VirtualAgents;
using UnityEngine;
using i5.VirtualAgents.ScheduleBasedExecution;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class ChatSampleController : SampleScheduleController
    {
        public Transform[] waypoints;

        protected override void Start()
        {
            base.Start();
            taskSystem.Tasks.GoTo(waypoints[0]);
            taskSystem.Tasks.WaitForSeconds(2f);
            taskSystem.Tasks.GoTo(waypoints[1]);
        }
        public void doWave()
        {
            AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
        }
        public void doHeadshake()
        {
            AgentBaseTask headShake = taskSystem.Tasks.PlayAnimation("ShakeHead", 5, "", 0, "Head");
        }
    }
}