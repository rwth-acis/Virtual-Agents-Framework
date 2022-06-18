using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class TaskBundleSampleController : MonoBehaviour
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
            IAgentTask wait = new AgentWaitTask(3);
            agent.ScheduleTask(wait, 0, "Left Arm");

            // These tasks will be configurated to start and end simultaniously.
            // Therefore, the second wave will only start once the first one and the head shaking has stopped

            IAgentTask waveTask = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(waveTask, 0, "Left Arm");

            IAgentTask shakeHeadTask = new AgentAnimationTask("ShakeHead", 10);
            agent.ScheduleTask(shakeHeadTask, 0, "Head");

            IAgentTask wave2Task = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(wave2Task, 0, "Left Arm");

            agent.DefineTaskDependency(wave2Task, waveTask, shakeHeadTask);

            //TODO show short way via shortcuts once implemented
        }
    }
}