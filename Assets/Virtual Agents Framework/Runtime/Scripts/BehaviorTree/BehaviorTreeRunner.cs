using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.BehaviourTrees.Visual;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Executes a given behaviour tree until the root node reports sucess or failure.
    /// </summary>
    public class BehaviorTreeRunner : TaskSystem
    {
        private Agent excecutingAgent;
        private ITask abstractTree;
        private TaskState rootState;

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

        public override void UpdateTaskSystem()
        {
            if (rootState != TaskState.Success && rootState != TaskState.Failure)
            {
                rootState = abstractTree.FullUpdate(excecutingAgent);
            }
        }
    }
}
