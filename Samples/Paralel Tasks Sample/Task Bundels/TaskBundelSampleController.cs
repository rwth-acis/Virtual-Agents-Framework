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
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.GoTo(waypoints[i].position);
            }

            //Long way:

            //Schedule 3 Seconds wait on arm layer
            IAgentTask wait = new AgentWaitTask(2);
            agent.ScheduleTask(wait, 0, "Left Arm");

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //These tasks will be configurated to start and end simultaniously.
            //Therefore, shaking the head will not start immediately but it will only start when the waving starts which was delayed by 2 seconds by the waiting task.
            //The manual way to schedule a bundle.

            TaskSynchroniser synchroniserStart = new TaskSynchroniser();
            TaskSynchroniser synchroniserEnd = new TaskSynchroniser();

            IAgentTask animationTask = new AgentAnimationTask("Wave", 5);
            animationTask.ReadyToStart = new List<System.Func<bool>>();
            animationTask.ReadyToStart.Add(synchroniserStart.WaitForOtherTasksMutually(animationTask));
            animationTask.ReadyToEnd = new List<System.Func<bool>>();
            animationTask.ReadyToEnd.Add(synchroniserEnd.WaitForOtherTasksMutually(animationTask));
            agent.ScheduleTask(animationTask, 0, "Left Arm");

            animationTask = new AgentAnimationTask("ShakeHead", 10);
            animationTask.ReadyToStart = new List<System.Func<bool>>();
            animationTask.ReadyToStart.Add(synchroniserStart.WaitForOtherTasksMutually(animationTask));
            animationTask.ReadyToEnd = new List<System.Func<bool>>();
            animationTask.ReadyToEnd.Add(synchroniserEnd.WaitForOtherTasksMutually(animationTask));
            agent.ScheduleTask(animationTask, 0, "Head");

            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            agent.Tasks.WaitForSeconds(1, default, "Head");

            //The same bundle, but scheduled via shortcuts
            
            agent.Tasks.OpenNewBundle();
            agent.Tasks.PlayAnimation("Wave", 5, "", 0, "Left Arm");
            agent.Tasks.PlayAnimation("ShakeHead", 10, "", 0, "Head");
            agent.Tasks.CloseBundle();


            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            //Wave on arm layer. Will only start once the headshaking from the interlocked tasks is finished, since the provious wave task will block the arm lane until all other interlocked tasks are finished------
            //animationTask = new AgentAnimationTask("Wave", 5);
            //agent.ScheduleTask(animationTask, 0, "Left Arm");
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            //TODO show short way via shortcuts once implemented
        }
    }
}