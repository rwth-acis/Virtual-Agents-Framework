using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentRotationController : SampleScheduleController
    {
        public List<Transform> waypoints;
        public Transform highPrioWaypoint;

        protected override void Start()
        {
            base.Start();
            // Using Rotate Right animation to rotate right

            AgentRotationTask rotation1= new AgentRotationTask(waypoints[1].gameObject);
            AgentRotationTask rotation2= new AgentRotationTask(waypoints[2].position);
            AgentRotationTask rotation3= new AgentRotationTask(90 );


            AgentRotationTask rotation4= new AgentRotationTask(waypoints[2].position);

            taskSystem.Tasks.WaitForSeconds(2, 0 );
            taskSystem.ScheduleTask(rotation1, 0, "Base Layer");
            taskSystem.Tasks.WaitForSeconds(1, 0 );
            taskSystem.Tasks.GoTo(highPrioWaypoint.gameObject, Vector3.zero,0, false);
            taskSystem.ScheduleTask(rotation2, 0, "Base Layer");
            taskSystem.Tasks.WaitForSeconds(1, 0 );
            taskSystem.ScheduleTask(rotation3, 0, "Base Layer");

            // Using Rotate Right animation to rotate left



            // Using Rotate Left animation to rotate right

            // Using Rotate Left animation to rotate left
        }
    }
}