using i5.VirtualAgents.AgentTasks;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AimingController : SampleScheduleController
    {
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;
        public bool useTaskShortcuts;
        public bool walk = true;
        public GameObject target;
        public int aimAtTime = 5;

        public bool aimHead = true;
        public bool aimLeftArm = true;
        public bool aimRightArm = true;
        public bool waveRightArm = true;

        protected override void Start()
        {
            base.Start();
            if (walk)
            {
                for (int i = 0; i < waypoints.Count; i++)
                {
                    taskSystem.Tasks.GoTo(waypoints[i].position);
                }
            }


            if (useTaskShortcuts)
            {
                Debug.Log("Agent has been equipped with tasks via shortcuts");

                // The quickest and most intuitive way is to use the task shortcuts of the agent
                if (waveRightArm)
                {
                    AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("WaveRight", 2, "", 0, "Right Arm");
                }

                //AgentBaseTask wave2 = taskSystem.Tasks.PlayAnimation("WaveLeft", 2, "", 0, "Left Arm");

                // A target can be added to all animations but works best with no animation or animations that are meant for aiming (eg. "startAimAt") 

                if (aimHead)
                {
                    AgentBaseTask pointingHead = taskSystem.Tasks.PlayAnimation("NoAnimation", aimAtTime, "", 0, "Head", target);
                }
                if (aimLeftArm)
                {
                    AgentBaseTask pointingLeft = taskSystem.Tasks.PlayAnimation("PointingLeft", aimAtTime, "", 0, "Left Arm", target);
                }
                if (aimRightArm)
                {
                    AgentBaseTask pointingRight = taskSystem.Tasks.PlayAnimation("PointingRight", aimAtTime, "", 0, "Right Arm", target);
                }



            }
            else
            {
                // Alternative way: Create the tasks and schedule them explicitly

                AgentAnimationTask pointingRight = new AgentAnimationTask("PointingRight", 10, "", target, "Right Arm");
                taskSystem.ScheduleTask(pointingRight, aimAtTime, "Right Arm");

                AgentAnimationTask pointingLeft = new AgentAnimationTask("PointingLeft", 10, "", target, "Left Arm");
                taskSystem.ScheduleTask(pointingLeft, aimAtTime, "Left Arm");

                AgentAnimationTask pointingHead = new AgentAnimationTask("NoAnimation", 10, "", target, "Head");
                taskSystem.ScheduleTask(pointingHead, aimAtTime, "Head");

            }
        }
    }
}