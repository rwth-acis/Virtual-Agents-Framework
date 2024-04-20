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

            taskSystem.ScheduleTask(new AgentMovementTask(waypoints[0].position ),1);
            Debug.Log("Running TaskBundleController");
            //Create a list of tasks
            List<AgentBaseTask> tasks = new List<AgentBaseTask>();

            //Create a list of preconditions
            List<System.Func<bool>> preconditions = new List<System.Func<bool>>();
            //preconditions.Add(true);
            //preconditions.Add(waypoints[0].position != agent.transform.position);
            preconditions.Add(() =>
            {
                Debug.Log("precondition 1");
                //Compare vectors equality with accuracy of 0.5

                float distance = Vector3.Distance(waypoints[0].position, agent.transform.position);
                Debug.Log("Distance: " + distance);
                Debug.Log(distance > 1.0f);
                return distance <= 1.0f;
            });

            //Add Tasks to the tasks List
            //tasks.Add(new AgentAnimationTask("WaveRight", 5));
            for (int i = 0; i < waypoints.Count; i++)
            {
                tasks.Add(new AgentMovementTask(waypoints[i].position));
            }

            //Create a new TaskBundle
            TaskBundle taskBundle = new TaskBundle(this, tasks, preconditions);

            // Schedule the TaskBundle to the taskSystem
            //taskSystem.Tasks.GoTo(waypoints[0].position);
            Debug.Log("before ScheduleTask");
            taskSystem.ScheduleTask(taskBundle, 0);
            Debug.Log("after ScheduleTask");

        }
    }
}