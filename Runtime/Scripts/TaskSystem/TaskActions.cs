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
        /// Creates an AgentMovementTask for walking and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationCoordinates">Position the agent should walk to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WalkTo(Vector3 destinationCoordinates, bool asap = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            agent.ScheduleOrForce(movementTask, asap);
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should walk to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WalkTo(GameObject destinationObject, bool asap = false)
        {
            WalkTo(destinationObject.transform.position, asap);
        }

        /// <summary>
        /// Creates an AgentMovementTask for walking and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">Transform the agent should walk to</param>
        /// <param name="asap">true if the task should be executed as soon as possible, false if the task should be scheduled</param>
        public void WalkTo(Transform destinationTransform, bool asap = false)
        {
            WalkTo(destinationTransform.position, asap);
        }
    }
}
