using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;
using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents.Examples
{
    public class TaskBundelSampleController : MonoBehaviour
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
            IAgentTask wait = new AgentWaitTask(2);
            agent.ScheduleTask(wait, 0, "Left Arm");

            //Task Bundel: Wave and shake head simultaniously---

            TaskSynchroniser synchroniserStart = new TaskSynchroniser();
            TaskSynchroniser synchroniserEnd = new TaskSynchroniser();

            IAgentTask animationTask = new AgentAnimationTask("Wave", 5);
            animationTask.PrepareSchedule = new List<System.Func<bool>>();
            animationTask.PrepareSchedule.Add(synchroniserStart.WaitForOtherTasksMutually(animationTask));
            animationTask.PrepareCleanup = new List<System.Func<bool>>();
            animationTask.PrepareCleanup.Add(synchroniserEnd.WaitForOtherTasksMutually(animationTask));
            agent.ScheduleTask(animationTask,0, "Left Arm");

            animationTask = new AgentAnimationTask("ShakeHead", 10);
            animationTask.PrepareSchedule = new List<System.Func<bool>>();
            animationTask.PrepareSchedule.Add(synchroniserStart.WaitForOtherTasksMutually(animationTask));
            animationTask.PrepareCleanup = new List<System.Func<bool>>();
            animationTask.PrepareCleanup.Add(synchroniserEnd.WaitForOtherTasksMutually(animationTask));
            agent.ScheduleTask(animationTask,0,"Head");

            //agent.ScheduleTaskBundel(tasks);
            //---------------------------------------------------

            //Wait and wave on arm layer------
            wait = new AgentWaitTask(2);
            agent.ScheduleTask(wait, 0, "Left Arm");

            animationTask = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(animationTask, 0, "Left Arm");
            //--------------------------------

            agent.Tasks.GoTo(highPrioWaypoint, Vector3.zero, 5);
        }
    }
}