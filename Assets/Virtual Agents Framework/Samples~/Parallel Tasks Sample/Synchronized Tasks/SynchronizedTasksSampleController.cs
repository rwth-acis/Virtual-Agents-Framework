using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.ScheduleBasedExecution;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class SynchronizedTasksSampleController : SampleScheduleController
    {
        /// <summary>
        /// List of waypoints which the agent should visit in order
        /// </summary>
        [Tooltip("List of waypoints which the agent should visit in order.")]
        public List<Transform> waypoints;

        /// <summary>
        /// Waypoint with a high priority
        /// </summary>
        [Tooltip("Waypoint with a high priority")]
        public Transform highPrioWaypoint;

        /// <summary>
        /// If true, the agent uses task shortcuts to create the tasks
        /// </summary>
        public bool useTaskShortcuts;

        /// <summary>
        /// If true, the agent walks to the waypoints. If false, the agent stays at the starting position.
        /// </summary>
        public bool walk = true;

        /// <summary>
        /// The time in seconds the agent should point at the target
        /// </summary>
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
                AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("WaveLeft", 5, "", 0, "Left Arm");
                AgentBaseTask headShake = taskSystem.Tasks.PlayAnimation("ShakeHead", 10, "", 0, "Left Arm");

                // Wave again but wait for the head shaking to end.
                // Implicitly, this also waits for the first waving animation to end
                // but we do not need to define that dependency as it is scheduled on the same layer
                taskSystem.Tasks.PlayAnimation("WaveLeft", 5, "", 0, "Left Arm")
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
                AgentAnimationTask waveTask = new AgentAnimationTask("WaveLeft", 5);
                taskSystem.ScheduleTask(waveTask, 0, "Left Arm");

                AgentAnimationTask shakeHeadTask = new AgentAnimationTask("ShakeHead", 10);
                taskSystem.ScheduleTask(shakeHeadTask, 0, "Head");

                // Wave again but wait for the head shaking to end.
                // Implicitly, this also waits for the first waving animation to end
                // but we do not need to define that dependency as it is scheduled on the same layer
                AgentAnimationTask wave2Task = new AgentAnimationTask("WaveLeft", 5);
                wave2Task.WaitFor(shakeHeadTask);
                taskSystem.ScheduleTask(wave2Task, 0, "Left Arm");
            }
        }
    }
}