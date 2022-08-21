using i5.VirtualAgents.AgentTasks;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.ScheduleBasedExecution
{
    /// <summary>
    /// List of shortcuts to schedule actions on an agent
    /// </summary>
    public class TaskActions
    {
        // the schedule system for which these tasks actions are
        private ScheduleBasedTaskSystem scheduleTaskSystem;

        /// <summary>
        /// Creates a new task actions shortcut and registers the schedule system on which the tasks are scheduled
        /// </summary>
        /// <param name="scheduleTaskSystem">The agent on which the tasks are scheduled and executed</param>
        public TaskActions(ScheduleBasedTaskSystem scheduleTaskSystem)
        {
            this.scheduleTaskSystem = scheduleTaskSystem;
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking/running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationCoordinates">Position the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public AgentBaseTask GoTo(Vector3 destinationCoordinates, int priority = 0)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            scheduleTaskSystem.ScheduleTask(movementTask, priority);
            return movementTask;
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking/running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">Transform the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public AgentBaseTask GoTo(Transform destinationTransform, Vector3 offset = default, int priority = 0)
        {
            return GoTo(destinationTransform.position + offset, priority);
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking/running and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public AgentBaseTask GoTo(GameObject destinationObject, Vector3 offset = default, int priority = 0)
        {
            return GoTo(destinationObject.transform, offset, priority);
        }

        /// <summary>
        /// Lets the agent wait for the given number of seconds in an idle position
        /// Shortcut queue management function
        /// </summary>
        /// <param name="seconds">The time span in seconds for which the agent shoudl wait</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="layer">The animation layer on which the task should be excuted</param>
        public AgentBaseTask WaitForSeconds(float seconds, int priority = 0, string layer = "Base Layer")
        {
            AgentWaitTask agentWaitTask = new AgentWaitTask(seconds);
            scheduleTaskSystem.ScheduleTask(agentWaitTask, priority, layer);
            return agentWaitTask;
        }


        /// <summary>
        /// Play an animation through the agents animation controller
        /// Shortcut queue management function
        /// </summary>
        /// <param name="startTrigger"></param> Trigger that starts the animation
        /// <param name="playTime"></param> Time in seconds after which the animation should stop
        /// <param name="stopTrigger"></param> Trigger that stops the animation. If not provided, start trigger is used again
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="layer"></param> The animation layer on which the task should be excuted
        public AgentBaseTask PlayAnimation(string startTrigger, float playTime, string stopTrigger = "", int priority = 0, string layer = "Base Layer")
        {
            AgentAnimationTask animationTask = new AgentAnimationTask(startTrigger, playTime, stopTrigger);
            scheduleTaskSystem.ScheduleTask(animationTask, priority, layer);
            return animationTask;
        }
    }
}
