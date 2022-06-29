using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class SynchronizedTasksSampleController : MonoBehaviour
    {
        public Agent agent;
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;
        public bool useTaskShortcuts;

        void Start()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.GoTo(waypoints[i].position);
            }

            if (useTaskShortcuts)
            {
                Debug.Log("Agent has been equipped with tasks via shortcuts");

                // The quickest and most intuitive way is to use the task shortcuts of the agent

                // Schedule 2 seconds wait on arm layer
                agent.Tasks.WaitForSeconds(2, 0, "Left Arm");

                // Wave and shake the head
                // The waving will start after the two second wait and the head shake will start immediately
                AgentBaseTask wave1 = agent.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
                AgentBaseTask headShake = agent.Tasks.PlayAnimation("ShakeHead", 10, "", 0, "Left Arm");

                // Wave again but wait for the first wave and the head shaking to end.
                agent.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm")
                    .WaitFor(wave1, headShake);
            }
            else
            {
                // Alternative way: Create the tasks and schedule them explicitly

                // Schedule 2 seconds wait on arm layer
                IAgentTask wait = new AgentWaitTask(2);
                agent.ScheduleTask(wait, 0, "Left Arm");

                // The second wave waits for the first wave and the head shaking to end.

                // Wave and shake the head
                // The waving will start after the two second wait and the head shake will start immediately
                AgentAnimationTask waveTask = new AgentAnimationTask("Wave", 5);
                agent.ScheduleTask(waveTask, 0, "Left Arm");

                AgentAnimationTask shakeHeadTask = new AgentAnimationTask("ShakeHead", 10);
                agent.ScheduleTask(shakeHeadTask, 0, "Head");

                // Wave again but wait for the first wave and the head shaking to end.
                AgentAnimationTask wave2Task = new AgentAnimationTask("Wave", 5);
                wave2Task.WaitFor(waveTask, shakeHeadTask);
                agent.ScheduleTask(wave2Task, 0, "Left Arm");
            }
        }
    }
}