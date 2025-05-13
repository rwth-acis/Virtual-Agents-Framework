using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentSittingController : SampleScheduleController
    {
        [SerializeField] GameObject Chair = null;
        [SerializeField] GameObject Stool = null;
        protected override void Start()
        {
            base.Start();
            AgentSittingTask sittingTask = new AgentSittingTask(Chair, SittingDirection.SITDOWN);
            AgentSittingTask standingTask = new AgentSittingTask(Chair, SittingDirection.STANDUP);

            // Notice that both tasks use "TOGGLE" as the direction, so the agent will sit down when standing and vice versa
            AgentSittingTask sittingTaskStool = new AgentSittingTask(Stool, SittingDirection.TOGGLE);
            AgentSittingTask standingTaskStool = new AgentSittingTask(Stool, SittingDirection.TOGGLE);

            GameObject destination1 = Chair.transform.Find("FeetPosition").gameObject;
            taskSystem.Tasks.GoTo(destination1, Vector3.zero, 0, true);
            taskSystem.Tasks.WaitForSeconds(1);
            // add a sitting task
            taskSystem.ScheduleTask(sittingTask);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.ScheduleTask(standingTask);

            GameObject destination = Stool.transform.Find("FeetPosition").gameObject;
            taskSystem.Tasks.GoTo(destination, Vector3.zero, 0, true);
            taskSystem.Tasks.WaitForSeconds(1);
            taskSystem.ScheduleTask(sittingTaskStool);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.ScheduleTask(standingTaskStool);
        }

    }
}