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
            //Create a list of tasks
            List<IAgentTask> tasks = new List<IAgentTask>();

            //Create a list of preconditions
            List<bool> preconditions = new List<bool>();
            Vector3 agentPosition = agent.transform.position;
            //preconditions.Add(waypoints[0].position != agentPosition);

        //Add Tasks to the tasks List
            tasks.Add(new AgentAnimationTask("WaveRight", 5));
            for (int i = 0; i < waypoints.Count; i++)
            {
                tasks.Add(new AgentMovementTask(waypoints[i].position));
            }

            //Create a new TaskBundle
            TaskBundle taskBundle = new TaskBundle(tasks, preconditions);

            // Schedule the TaskBundle to the taskSystem
            //taskSystem.Tasks.GoTo(waypoints[0].position);
            taskSystem.ScheduleTask(taskBundle);

        }
    }
}