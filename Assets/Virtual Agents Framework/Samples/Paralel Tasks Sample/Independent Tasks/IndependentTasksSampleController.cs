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
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut but we could also just create a new
            // AgentMovementTask and schedule it using agent.ScheduleTask.
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.GoTo(waypoints[i].position);
            }

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

            // example for a different priority:
            // this waypoint is added last but has the highest priority,
            // so the agent will walk to it first
            agent.Tasks.GoTo(highPrioWaypoint, Vector3.zero, 5);
        }
    }
}