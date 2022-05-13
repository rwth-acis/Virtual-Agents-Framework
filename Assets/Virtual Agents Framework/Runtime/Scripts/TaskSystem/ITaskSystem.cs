using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public interface ITaskSystem
    {
        void Update();
        void ScheduleTask(IAgentTask task, int priority = 0, string layer = "Base Layer");
    }
}
