using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class TaskBundleController : SampleScheduleController
    {
        public List<Transform> waypoints;

        protected override void Start()
        {
            base.Start();
            // Create a TaskBundle consisting of a task list and a list of preconditions

            // Create a list of tasks
            List<AgentBaseTask> tasks = new List<AgentBaseTask>();

            // Create a list of preconditions
            List<System.Func<bool>> preconditions = new List<System.Func<bool>>();

            // Preconditions can be added with a lambda function
            // The lambda function returns a boolean value
            // () => {...} syntax is a lambda expression that defines an anonymous function inline.
            // Parameters can be added in brackets on the left side of the arrow
            preconditions.Add(() =>
            {
                // This lambda function checks if the agent is close to the last waypoint
                // If a precondition is not met, the TaskBundle will not be executed
                // Compare vectors equality with accuracy of 0.5
                float distance = Vector3.Distance(waypoints[waypoints.Count - 1].position, agent.transform.position);
                return distance > 1.0f;
            });

            // Add Tasks to the tasks List before creating the TaskBundle
            // TaskActions also add the task to the taskSystem therefore cannot be used here
            for (int i = 0; i < waypoints.Count; i++)
            {
                tasks.Add(new AgentMovementTask(waypoints[i].position));
            }

            // Create a new TaskBundle with the tasks and preconditions
            // This TaskBundle will be executed because the agent is not close to the last waypoint
            TaskBundle taskBundleSuccess = new TaskBundle(tasks, preconditions);

            // Create a TaskBundle with empty tasks and preconditions first
            // This TaskBundle will not be executed
            // because after executing the taskBundleSuccess the agent is close to the last waypoint
            TaskBundle taskBundleFail = new TaskBundle();

            // Add Tasks to the tasks List after creating the TaskBundle
            for (int i = 0; i < waypoints.Count; i++)
            {
                taskBundleFail.AddTask(new AgentMovementTask(waypoints[i].position));
            }

            // Add the same precondition to the second TaskBundle, but after the TaskBundle was created
            taskBundleFail.AddPrecondition( () =>
            {
                float distance = Vector3.Distance(waypoints[waypoints.Count - 1].position, agent.transform.position);
                return distance > 1.0f;
            });


            // Schedule the TaskBundles to the taskSystem
            taskSystem.ScheduleTask(taskBundleSuccess, 0);
            taskSystem.ScheduleTask(taskBundleFail, 0);
            Debug.Log("Two TaskBundles scheduled. The first one should succeed, the second one should fail.");
        }
    }
}