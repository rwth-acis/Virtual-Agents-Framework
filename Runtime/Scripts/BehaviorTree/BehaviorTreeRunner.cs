using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public class BehaviorTreeRunner : ITaskSystem
    {
        Agent excecutingAgent;
        public INode behaviorTreeRoot;

        public BehaviorTreeRunner(Agent excecutingAgent)
        {
            this.excecutingAgent = excecutingAgent;
        }

        public void ScheduleTask(IAgentTask task, int priority = 0, string layer = "Base Layer")
        {
            throw new System.NotImplementedException();
        }

        void ITaskSystem.Update()
        {
            behaviorTreeRoot?.FullUpdate(excecutingAgent);
        }
    }
}
