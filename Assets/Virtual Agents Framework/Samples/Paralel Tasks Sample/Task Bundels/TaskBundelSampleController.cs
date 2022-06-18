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

            //These tasks will be configurated to start and end simultaniously.
            //Therefore, shaking the head will not start immediately but it will only start when the waving starts which was delayed by 2 seconds by the waiting task--------------------

            IAgentTask waveTask = new AgentAnimationTask("Wave", 5);
            agent.ScheduleTask(waveTask, 0, "Left Arm");

            IAgentTask shakeHeadTask = new AgentAnimationTask("ShakeHead", 10);
            agent.ScheduleTask(shakeHeadTask, 0, "Head");

            IAgentTask wave2Task = new AgentAnimationTask("Wave", 5);

            agent.DefineTaskDependency(wave2Task, waveTask, shakeHeadTask);

            agent.ScheduleTask(wave2Task, 0, "Left Arm");

            //TaskSynchronizer synchroniserStart = new TaskSynchronizer();
            //TaskSynchronizer synchroniserEnd = new TaskSynchronizer();

            //IAgentTask animationTask = new AgentAnimationTask("Wave", 5);
            //animationTask.ReadyToStart = new List<System.Func<bool>>();
            //animationTask.ReadyToStart.Add(synchroniserStart.WaitForOtherTasksMutually(animationTask));
            //animationTask.ReadyToEnd = new List<System.Func<bool>>();
            //animationTask.ReadyToEnd.Add(synchroniserEnd.WaitForOtherTasksMutually(animationTask));
            //agent.ScheduleTask(animationTask,0, "Left Arm");

            //animationTask = new AgentAnimationTask("ShakeHead", 10);
            //animationTask.ReadyToStart = new List<System.Func<bool>>();
            //animationTask.ReadyToStart.Add(synchroniserStart.WaitForOtherTasksMutually(animationTask));
            //animationTask.ReadyToEnd = new List<System.Func<bool>>();
            //animationTask.ReadyToEnd.Add(synchroniserEnd.WaitForOtherTasksMutually(animationTask));
            //agent.ScheduleTask(animationTask,0,"Head");
            ////---------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            ////Wave on arm layer. Will only start once the headshaking from the interlocked tasks is finished, since the provious wave task will block the arm lane until all other interlocked tasks are finished------
            //animationTask = new AgentAnimationTask("Wave", 5);
            //agent.ScheduleTask(animationTask, 0, "Left Arm");
            ////---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


            //TODO show short way via shortcuts once implemented
        }
    }
}