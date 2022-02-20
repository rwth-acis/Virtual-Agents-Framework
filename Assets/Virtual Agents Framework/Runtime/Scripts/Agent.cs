using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;
using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Agent's functionality mainly includes managing their task queue,
    /// responding to task execution statuses and changing one's state accordingly
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))] // Responsible for the proxy object's movement
    [RequireComponent(typeof(AgentAnimationUpdater))] // Responsible for the avatar's movement
    public class Agent : MonoBehaviour
    {
        AgentTaskManager taskManager;

        /// <summary>
        /// Initialize the agent
        /// </summary>
        private void Awake()
        {
            taskManager = new AgentTaskManager(this);
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        private void Update()
        {
            taskManager.Update();
        }

        /// <summary>
        /// Schedule a task
        /// </summary>
        /// <param name="task">Task to be scheduled</param>
        public void ScheduleTask(IAgentTask task)
        {
            taskManager.ScheduleTask(task);
        }

        /// <summary>
        /// Execute a task as soon as possible
        /// </summary>
        /// <param name="task">Task to be executed</param>
        public void ExecuteTaskASAP(IAgentTask task)
        {
            taskManager.ForceTask(task);
        }

        /// <summary>
        /// Helper function for shortcut queue management functions.
        /// Schedule a task or force its execution depending on the flag
        /// </summary>
        /// <param name="task">Task to be scheduled or forced</param>
        /// <param name="force">Flag: true if the task's execution should be forced, false if the task should be scheduled</param>
        public void ScheduleOrForce(IAgentTask task, bool force)
        {
            if (force == true)
            {
                taskManager.ForceTask(task);
            }
            else
            {
                taskManager.ScheduleTask(task);
            }
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
            ScheduleOrForce(movementTask, asap);
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
