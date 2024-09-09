using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentSittingController : SampleScheduleController
    {
        [SerializeField] GameObject Chair = null;
        protected override void Start()
        {
            base.Start();
            AgentSittingTask sittingTask = new AgentSittingTask(Chair, SittingDirection.SITDOWN);
            AgentSittingTask standingTask = new AgentSittingTask(Chair, SittingDirection.STANDUP);
            //AgentSittingTask standUpTask = new AgentSittingTask(true);
            // add a sitting task
            taskSystem.ScheduleTask(sittingTask);
            taskSystem.Tasks.WaitForSeconds(3);
            Debug.Log("2nd Sitting Task Scheduled");
            taskSystem.ScheduleTask(standingTask);
        }

    }
}