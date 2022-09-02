using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.ScheduleBasedExecution;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class IndependentTasksSampleController : SampleScheduleController
    {
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;
        public bool useTaskShortcuts = true;

        public override void Start()
        {
            base.Start();
            for (int i = 0; i < waypoints.Count; i++)
            {
                taskSystem.Tasks.GoTo(waypoints[i].position);
            }

            if (useTaskShortcuts)
            {
                Debug.Log("Agent has been equipped with tasks via shortcuts");

                // The quickest and most intuitive way is to use the task shortcuts of the agent

                // Schedule 3 Seconds wait on arm layer
                taskSystem.Tasks.WaitForSeconds(3, 0, "Left Arm");

                // Schedule waving for 5 seconds on arm layer
                taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");

                // Schedule shake head for 5 seconds on head layer
                taskSystem.Tasks.PlayAnimation("ShakeHead", 5, "", 0, "Head");

                // Wait and wave on arm layer
                taskSystem.Tasks.WaitForSeconds(2, 0, "Left Arm");
                taskSystem.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
            }
            else
            {
                // Alternative way: create the tasks and schedule them explicitly
                Debug.Log("Agent has been equipped with tasks via task scheduling without shortcuts");

                // Schedule 3 Seconds wait on arm layer
                IAgentTask wait = new AgentWaitTask(3);
                taskSystem.ScheduleTask(wait, 0, "Left Arm");

                // Schedule waving for 5 seconds on arm layer
                IAgentTask animationTask = new AgentAnimationTask("Wave", 5);
                taskSystem.ScheduleTask(animationTask, 0, "Left Arm");

                // Schedule shake head for 5 seconds on head layer
                animationTask = new AgentAnimationTask("ShakeHead", 5);
                taskSystem.ScheduleTask(animationTask, 0, "Head");

                // Wait and wave on arm layer
                wait = new AgentWaitTask(2);
                taskSystem.ScheduleTask(wait, 0, "Left Arm");

                animationTask = new AgentAnimationTask("Wave", 5);
                taskSystem.ScheduleTask(animationTask, 0, "Left Arm");
            }
        }
    }
}