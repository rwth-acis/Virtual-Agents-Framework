using i5.VirtualAgents.TaskSystem.AgentTasks;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// List of shortcuts to schedule actions on an agent
    /// </summary>
    public class TaskActions
    {
        // the agent for which these tasks actions are
        private Agent agent;

        //Synchroniser for managing task bundles
        private TaskSynchroniser synchroniserStart;
        private TaskSynchroniser synchroniserEnd;

        /// <summary>
        /// Creates a new task actions shortcut and registers the agent on which the tasks are scheduled
        /// </summary>
        /// <param name="agent">The agent on which the tasks are scheduled and executed</param>
        public TaskActions(Agent agent)
        {
            this.agent = agent;
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking/running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationCoordinates">Position the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public void GoTo(Vector3 destinationCoordinates, int priority = 0)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            ScheduleWithRespectTobundle(movementTask, priority);
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking/running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">Transform the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public void GoTo(Transform destinationTransform, Vector3 offset = default, int priority = 0)
        {
            GoTo(destinationTransform.position + offset, priority);
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking/running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public void GoTo(GameObject destinationObject, Vector3 offset = default, int priority = 0)
        {
            GoTo(destinationObject.transform, offset, priority);
        }

        /// <summary>
        /// Lets the agent wait for the given number of seconds in an idle position
        /// Shortcut queue management function
        /// </summary>
        /// <param name="seconds">The time span in seconds for which the agent shoudl wait</param>
        /// <param name="layer">The animation layer on which the task should be excuted</param>
        public void WaitForSeconds(float seconds, int priority = 0, string layer = "Base Layer")
        {
            ScheduleWithRespectTobundle(new AgentWaitTask(seconds), priority, layer);
        }


        /// <summary>
        /// Play an animation through the agents animation controller
        /// Shortcut queue management function
        /// </summary>
        /// <param name="startTrigger"></param> Trigger that starts the animation
        /// <param name="playTime"></param> Time in seconds after which the animation should stop
        /// <param name="stopTrigger"></param> Trigger that stops the animation. If not provided, start trigger is used again
        /// <param name="priortiy"></param>
        /// <param name="layer"></param> The animation layer on which the task should be excuted
        public void PlayAnimation(string startTrigger, float playTime, string stopTrigger = "", int priortiy = 0, string layer = "Base Layer")
        {
            IAgentTask animationTask = new AgentAnimationTask(startTrigger,playTime,stopTrigger);
            ScheduleWithRespectTobundle(animationTask,priortiy,layer);
        }


        /// <summary>
        /// Every task that will be scheduled through the task actions after opening a bundle and before closing it will automatically be added to taht bundle.
        /// Task that are part of a bundle will wait with starting/ending until all other task from the budle are ready to start/end
        /// </summary>
        /// <param name="waitForStart"></param> Should tasks wait with starting for all other tasks from the bundle
        /// <param name="waitForEnd"></param>Should tasks wait with ending for all other tasks from the bundle. Waiting for end will not elongate the task itself, only block the respective layer longer.
        /// <returns></returns>
        public bool OpenNewBundle(bool waitForStart = true, bool waitForEnd = true)
        {
            CloseBundle();
            if (!waitForStart && !waitForEnd)
                //The bundle has to wait for start or end, otherwise it doesn't perform any work
                return false;
            if (waitForStart)
                synchroniserStart = new TaskSynchroniser();
            if (waitForEnd)
                synchroniserEnd = new TaskSynchroniser();
            return true;
        }

        /// <summary>
        /// Afte closing, tasks can be scheduled normal again.
        /// </summary>
        public void CloseBundle()
        {
            synchroniserStart = null;
            synchroniserEnd = null;
        }

        /// <summary>
        /// Schedule task and add it to the current bundle, if there is a bundle open currently.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="priortiy"></param>
        /// <param name="layer"></param> The animation layer on which the task should be excuted
        public void ScheduleWithRespectTobundle(IAgentTask task, int priortiy = 0, string layer = "Base Layer")
        {
            if(synchroniserStart != null)
            {
                if (task.ReadyToStart == null)
                {
                    task.ReadyToStart = new List<System.Func<bool>>();
                }
                task.ReadyToStart.Add(synchroniserStart.WaitForOtherTasksMutually(task));
            }
            if (synchroniserEnd != null)
            {
                if (task.ReadyToEnd == null)
                {
                    task.ReadyToEnd = new List<System.Func<bool>>();
                }
                task.ReadyToEnd.Add(synchroniserEnd.WaitForOtherTasksMutually(task));
            }
            agent.ScheduleTask(task,priortiy,layer);
        }
    }
}
