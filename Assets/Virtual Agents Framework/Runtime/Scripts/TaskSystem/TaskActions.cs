using i5.VirtualAgents.TaskSystem.AgentTasks;
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
            agent.ScheduleTask(movementTask, priority);
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
    }
}
