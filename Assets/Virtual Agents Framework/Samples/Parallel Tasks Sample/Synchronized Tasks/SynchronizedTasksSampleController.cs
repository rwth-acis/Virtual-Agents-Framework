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

        void Start()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.GoTo(waypoints[i].position);
            }

            // Long way:

            // Schedule 3 Seconds wait on arm layer
            IAgentTask wait = new AgentWaitTask(2);
            agent.ScheduleTask(wait, 0, "Left Arm");

            // The second wave waits for the first wave and the head shaking to end.

            AgentAnimationTask waveTask = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(waveTask, 0, "Left Arm");

            AgentAnimationTask shakeHeadTask = new AgentAnimationTask("ShakeHead", 10);
            agent.ScheduleTask(shakeHeadTask, 0, "Head");

            AgentAnimationTask wave2Task = new AgentAnimationTask("Wave", 5);
            wave2Task.WaitFor(waveTask, shakeHeadTask);
            agent.ScheduleTask(wave2Task, 0, "Left Arm");

            //TODO show short way via shortcuts once implemented
        }
    }
}