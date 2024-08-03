using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentRotationController : SampleScheduleController
    {
        public List<Transform> waypoints;

        protected override void Start()
        {
            base.Start();
            // Rotate towards a target object
            AgentRotationTask rotationTarget= new AgentRotationTask(waypoints[1].gameObject);

            // Rotate towards a coordinate
            AgentRotationTask rotationCoordinate= new AgentRotationTask(waypoints[2].position);

            // Rotate by a specific angle
            AgentRotationTask rotationAngle1= new AgentRotationTask(45);

            // Change the rotation value of the agent to a specific angle
            AgentRotationTask rotationAngle2= new AgentRotationTask(90, false);

            taskSystem.Tasks.WaitForSeconds(1, 0);
            taskSystem.ScheduleTask(rotationTarget, 0, "Base Layer");
            taskSystem.Tasks.WaitForSeconds(1, 0);
            taskSystem.ScheduleTask(rotationCoordinate, 0, "Base Layer");
            taskSystem.Tasks.WaitForSeconds(1, 0);
            taskSystem.ScheduleTask(rotationAngle1, 0, "Base Layer");
            taskSystem.Tasks.WaitForSeconds(1, 0);
            taskSystem.ScheduleTask(rotationAngle2, 0, "Base Layer");
        }
    }
}