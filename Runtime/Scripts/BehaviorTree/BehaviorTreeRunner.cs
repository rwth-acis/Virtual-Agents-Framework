using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Executes a given behaviour tree as long as the root node doesn't reprot sucess or failure.
    /// </summary>
    public class BehaviorTreeRunner : ITaskSystem
    {
        Agent excecutingAgent;
        private ITask abstractTree;
        TaskState rootState;

        public BehaviorTreeRunner(Agent excecutingAgent, BehaviorTreeAsset tree)
        {
            this.excecutingAgent = excecutingAgent;
            abstractTree = tree.GetExecutableTree();
        }

        /// <summary>
        /// Scheduling aditional tasks in a behaviour tree is currently not supported
        /// </summary>
        /// <param name="task"></param>
        /// <param name="priority"></param>
        /// <param name="layer"></param>
        public void ScheduleTask(IAgentTask task, int priority = 0, string layer = "Base Layer")
        {
            //TODO Maby halt the tree execution, execute new task and then continue with the tree?
            throw new System.NotImplementedException();
        }

        void ITaskSystem.Update()
        {
            if (rootState != TaskState.Success && rootState != TaskState.Failure)
            {
                rootState = abstractTree.FullUpdate(excecutingAgent);
            }
        }
    }
}
