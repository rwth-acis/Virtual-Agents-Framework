using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class IndependentTasksSampleController : MonoBehaviour
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


            //Long way:

            //Schedule 3 Seconds wait on arm layer
            IAgentTask wait = new AgentWaitTask(3);
            agent.ScheduleTask(wait, 0, "Left Arm");

            //Schedule waving for 5 seconds on arm layer
            IAgentTask animationTask = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(animationTask, 0, "Left Arm");

            //Schedule shake head for 5 seconds on head layer
            animationTask = new AgentAnimationTask("ShakeHead", 5);
            agent.ScheduleTask(animationTask, 0, "Head");


            //Wait and wave on arm layer------
            wait = new AgentWaitTask(2);
            agent.ScheduleTask(wait, 0, "Left Arm");

            animationTask = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(animationTask, 0, "Left Arm");
            //--------------------------------

            //TODO show short way via shortcuts once implemented
        }
    }
}