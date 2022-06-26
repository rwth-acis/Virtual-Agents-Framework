using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Executes a given behaviour tree until the root node reports sucess or failure.
    /// </summary>
    public class BehaviorTreeRunner : ITaskSystem
    {
        Agent excecutingAgent;
        private ITask abstractTree;
        TaskState rootState;

        /// <summary>
        /// Execute visual behaviour tree.
        /// </summary>
        /// <param name="excecutingAgent"></param>
        /// <param name="tree"></param>
        public BehaviorTreeRunner(Agent excecutingAgent, BehaviorTreeAsset tree)
        {
            this.excecutingAgent = excecutingAgent;
            abstractTree = tree.GetExecutableTree();
        }

        /// <summary>
        /// Execute abstract behaviour tree.
        /// </summary>
        /// <param name="excecutingAgent"></param>
        /// <param name="rootNode"></param>
        public BehaviorTreeRunner(Agent excecutingAgent, ITask rootNode)
        {
            this.excecutingAgent = excecutingAgent;
            abstractTree = rootNode;
        }

        /// <summary>
        /// Scheduling aditional tasks in a behaviour tree is currently not supported.
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
