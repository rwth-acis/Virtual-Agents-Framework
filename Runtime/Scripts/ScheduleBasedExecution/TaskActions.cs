using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;
using static i5.VirtualAgents.MeshSockets;

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
        /// Creates an AgentMovementTask for walking/running to a specific GameObject and schedules it or forces its execution.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">GameObject the agent should go to</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="follow">Decides if the Agent should follow the GameObject, dynamically, even if the path cannot reach the GameObject</param>
        public AgentBaseTask GoTo(GameObject destinationObject, Vector3 offset = default, int priority = 0, bool follow = false)
        {
            if (follow)
            {
                AgentMovementTask movementTask = new AgentMovementTask(destinationObject, default, follow);
                scheduleTaskSystem.ScheduleTask(movementTask, priority);
                return movementTask;
            }
            else
            {
                return GoTo(destinationObject.transform, offset, priority);
            }
        }

        /// <summary>
        /// Lets the agent wait for the given number of seconds in an idle position
        /// Shortcut queue management function
        /// </summary>
        /// <param name="seconds">The time span in seconds for which the agent should wait</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="layer">The animation layer on which the task should be executed</param>
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
        /// <param name="startTrigger"> Trigger that starts the animation</param>
        /// <param name="playTime"> Time in seconds after which the animation should stop</param>
        /// <param name="stopTrigger"> Trigger that stops the animation. If not provided, start trigger is used again</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="layer"> The animation layer on which the task should be executed </param>
        /// <param name="aimTarget">The target at which the agent should aim while playing the animation</param>
        public AgentBaseTask PlayAnimation(string startTrigger, float playTime, string stopTrigger = "", int priority = 0, string layer = "Base Layer", GameObject aimTarget = null)
        {
            AgentAnimationTask animationTask = new AgentAnimationTask(startTrigger, playTime, stopTrigger, layer, aimTarget);
            scheduleTaskSystem.ScheduleTask(animationTask, priority, layer);
            return animationTask;
        }

        /// <summary>
        /// Pick up an object with the Item component that is currently in reach of the agent
        /// Shortcut queue management function
        /// </summary>
        /// <param name="pickupObject">Object that should be picked up. Needs to have an item component and be near to the agent.</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="bodyAttachPoint">Agent socket that the object should be attached to, standard is the right Hand</param>
        /// <returns></returns>
        public AgentBaseTask PickUp(GameObject pickupObject, int priority = 0, SocketId bodyAttachPoint = SocketId.RightHand)
        {
            AgentPickUpTask pickUpTask = new AgentPickUpTask(pickupObject, bodyAttachPoint);
            scheduleTaskSystem.ScheduleTask(pickUpTask, priority);
            return pickUpTask;
        }
        /// <summary>
        /// Go to an object with the Item component and pick it up when near enough.
        /// Might fail, when object is moving too fast.
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationObject">Object the agent should go to and pick up. Needs to have an item component and be reachable by the agent.</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <param name="bodyAttachPoint">Agent socket that the object should be attached to, standard is the right Hand</param>
        /// <param name="minDistance">Distance at which the the agent will try to pick up the object</param>
        /// <returns></returns>
        public AgentBaseTask GoToAndPickUp(GameObject destinationObject, int priority = 0, SocketId bodyAttachPoint = SocketId.RightHand, float minDistance = 0.3f)
        {
            List<AgentBaseTask> tasks = new List<AgentBaseTask>();
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject, default, true);
            movementTask.MinDistance = minDistance;
            tasks.Add(movementTask);
            AgentPickUpTask pickUpTask = new AgentPickUpTask(destinationObject, bodyAttachPoint);
            tasks.Add(pickUpTask);
            TaskBundle PickUpBundle = new TaskBundle(tasks);

            scheduleTaskSystem.ScheduleTask(PickUpBundle, priority);
            return PickUpBundle;
        }

        /// <summary>
        /// Drop one specified or all object that are currently attached to the agent and have the Item component
        /// Shortcut queue management function
        /// </summary>
        /// <param name="dropObject">The item that should be dropped, no item will result in all items being dropped</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <returns></returns>
        public AgentBaseTask DropItem(GameObject dropObject = null, int priority = 0)
        {
            AgentDropTask dropTask = new AgentDropTask(dropObject);
            scheduleTaskSystem.ScheduleTask(dropTask, priority);
            return dropTask;
        }
        /// <summary>
        /// Go to coordinates and drop one specified or all object that are currently attached to the agent and have the Item component
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationCoordinates">Position the agent should go to</param>
        /// <param name="dropObject">The item that should be dropped, no item will result in all items being dropped</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <returns></returns>
        public AgentBaseTask GoToAndDropItem(Vector3 destinationCoordinates, GameObject dropObject = null, int priority = 0)
        {
            List<AgentBaseTask> tasks = new List<AgentBaseTask>();
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            tasks.Add(movementTask);
            AgentDropTask dropTask = new AgentDropTask(dropObject);
            tasks.Add(dropTask);
            TaskBundle dropTaskBundle = new TaskBundle(tasks);

            scheduleTaskSystem.ScheduleTask(dropTaskBundle, priority);
            return dropTaskBundle;
        }
        /// <summary>
        /// Go to a transform and drop one specified or all object that are currently attached to the agent and have the Item component
        /// Shortcut queue management function
        /// </summary>
        /// <param name="destinationTransform">Transform the agent should go to</param>
        /// <param name="dropObject">The item that should be dropped, no item will result in all items being dropped</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <returns></returns>
        public AgentBaseTask GoToAndDropItem(Transform destinationTransform, GameObject dropObject = null, int priority = 0)
        {
            List<AgentBaseTask> tasks = new List<AgentBaseTask>();
            AgentMovementTask movementTask = new AgentMovementTask(destinationTransform.position);
            tasks.Add(movementTask);
            AgentDropTask dropTask = dropObject == null ? new AgentDropTask() : new AgentDropTask(dropObject);
            tasks.Add(dropTask);
            TaskBundle dropTaskBundle = new TaskBundle(tasks);

            scheduleTaskSystem.ScheduleTask(dropTaskBundle, priority);
            return dropTaskBundle;
        }

        /// <summary>
        /// Creates an adaptiveGazeTask that activates the AdaptiveGaze component, then a wait task on the head and then a task that deactivates the AdaptiveGaze component. These a scheduled so that they play one after another.
        /// </summary>
        /// <param name="seconds">Time in seconds after which the gazing should stop</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <returns>Returns a AgentBaseTask array with two elements. The first has the starting Task (e.g. for startTask.waitFor(differentTask), and the second the stop Task ((e.g. for differentTask.waitFor(stopTask))</returns>
        public AgentBaseTask[] StartAdaptiveGazeForTime(float seconds, int priority = 0)
        {
            //TODO: Check if this can better be solved with a TaskBundle
            AgentBaseTask adaptiveGazeTaskStart = new AgentAdaptiveGazeTask(true);
            scheduleTaskSystem.ScheduleTask(adaptiveGazeTaskStart, priority, "Head");
            AgentBaseTask waitHead = WaitForSeconds(seconds, priority, "Head");
            waitHead.WaitFor(adaptiveGazeTaskStart);
            AgentBaseTask adaptiveGazeTaskStop = new AgentAdaptiveGazeTask(false);
            adaptiveGazeTaskStop.WaitFor(waitHead);
            scheduleTaskSystem.ScheduleTask(adaptiveGazeTaskStop, priority, "Head");
            return new AgentBaseTask[] { adaptiveGazeTaskStart, adaptiveGazeTaskStop };
        }
        /// <summary>
        /// Creates an adaptiveGazeTask that activates or deactivates the AdaptiveGaze component on the agent
        /// </summary>
        /// <param name="shouldStartOrStop">If true, will start adaptive Gaze. If false will stop adaptive gaze</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        /// <returns></returns>
        public void ActivateOrDeactivateAdaptiveGaze(bool shouldStartOrStop, int priority = 0)
        {
            AgentBaseTask adaptiveGazeTask = new AgentAdaptiveGazeTask(shouldStartOrStop);
            scheduleTaskSystem.ScheduleTask(adaptiveGazeTask, priority, "Head");
        }
    }
}
