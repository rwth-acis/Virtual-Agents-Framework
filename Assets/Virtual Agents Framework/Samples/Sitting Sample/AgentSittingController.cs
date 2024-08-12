using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentSittingController : SampleScheduleController
    {
        protected override void Start()
        {
            base.Start();
            AgentSittingTask sittingTask = new AgentSittingTask(true);
            //AgentSittingTask standUpTask = new AgentSittingTask(true);
            // add a sitting task
            taskSystem.ScheduleTask(sittingTask);
            taskSystem.ScheduleTask(sittingTask);
        }

    }
}