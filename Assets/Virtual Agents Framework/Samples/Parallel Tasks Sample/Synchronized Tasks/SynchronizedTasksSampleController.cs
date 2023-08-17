using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.ScheduleBasedExecution;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class SynchronizedTasksSampleController : SampleScheduleController
    {
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;
        public bool useTaskShortcuts;
        public bool walk = true;
        public GameObject target;
        public int pointAtTime = 5;

        protected override void Start()
        {
            base.Start();
            if(walk)
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

                // Schedule 2 seconds wait on arm layer
                taskSystem.Tasks.WaitForSeconds(2, 0, "Left Arm");

                // Wave and shake the head
                // The waving will start after the two second wait and the head shake will start immediately
                AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
                AgentBaseTask headShake = taskSystem.Tasks.PlayAnimation("ShakeHead", 10, "", 0, "Left Arm");
                AgentBaseTask pointing= taskSystem.Tasks.PlayAnimation("startPointAt", pointAtTime, "stopPointAt", 0, "Right Arm");
                // Wave again but wait for the head shaking to end.
                // Implicitly, this also waits for the first waving animation to end
                // but we do not need to define that dependency as it is scheduled on the same layer
                taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm")
                    .WaitFor(headShake);
            }
            else
            {
                // Alternative way: Create the tasks and schedule them explicitly

                // Schedule 2 seconds wait on arm layer
                IAgentTask wait = new AgentWaitTask(2);
                taskSystem.ScheduleTask(wait, 0, "Left Arm");

                // The second wave waits for the first wave and the head shaking to end.

                // Wave and shake the head
                // The waving will start after the two second wait and the head shake will start immediately
                AgentAnimationTask waveTask = new AgentAnimationTask("Wave", 5);
                taskSystem.ScheduleTask(waveTask, 0, "Left Arm");

                AgentAnimationTask shakeHeadTask = new AgentAnimationTask("ShakeHead", 10);
                taskSystem.ScheduleTask(shakeHeadTask, 0, "Head");

                // Wave again but wait for the head shaking to end.
                // Implicitly, this also waits for the first waving animation to end
                // but we do not need to define that dependency as it is scheduled on the same layer
                AgentAnimationTask wave2Task = new AgentAnimationTask("Wave", 5);
                wave2Task.WaitFor(shakeHeadTask);
                taskSystem.ScheduleTask(wave2Task, 0, "Left Arm");
                AgentAnimationTask pointAt = new AgentAnimationTask("startPointAt", pointAtTime, "stopPointAt", target);
                taskSystem.ScheduleTask(pointAt, 0, "Right Arm");
            }
        }
    }
}