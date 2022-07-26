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
        public bool useTaskShortcuts = true;

        private void Start()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.GoTo(waypoints[i].position);
            }

            if (useTaskShortcuts)
            {
                Debug.Log("Agent has been equipped with tasks via shortcuts");

                // The quickest and most intuitive way is to use the task shortcuts of the agent

                // Schedule 3 Seconds wait on arm layer
                agent.Tasks.WaitForSeconds(3, 0, "Left Arm");

                // Schedule waving for 5 seconds on arm layer
                agent.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");

                // Schedule shake head for 5 seconds on head layer
                agent.Tasks.PlayAnimation("ShakeHead", 5, "", 0, "Head");

                // Wait and wave on arm layer
                agent.Tasks.WaitForSeconds(2, 0, "Left Arm");
                agent.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
            }
            else
            {
                // Alternative way: create the tasks and schedule them explicitly
                Debug.Log("Agent has been equipped with tasks via task scheduling without shortcuts");

                // Schedule 3 Seconds wait on arm layer
                IAgentTask wait = new AgentWaitTask(3);
                agent.ScheduleTask(wait, 0, "Left Arm");

                // Schedule waving for 5 seconds on arm layer
                IAgentTask animationTask = new AgentAnimationTask("Wave", 5);
                agent.ScheduleTask(animationTask, 0, "Left Arm");

                // Schedule shake head for 5 seconds on head layer
                animationTask = new AgentAnimationTask("ShakeHead", 5);
                agent.ScheduleTask(animationTask, 0, "Head");

                // Wait and wave on arm layer
                wait = new AgentWaitTask(2);
                agent.ScheduleTask(wait, 0, "Left Arm");

                animationTask = new AgentAnimationTask("Wave", 5);
                agent.ScheduleTask(animationTask, 0, "Left Arm");
            }
        }
    }
}