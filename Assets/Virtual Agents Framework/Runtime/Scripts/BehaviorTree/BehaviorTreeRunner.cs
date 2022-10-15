using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.BehaviourTrees.Visual;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes a given behaviour tree until the root node reports sucess or failure. Can either be provided with a Behaviour Tree Asset or can be given an AbstractTree manually constructed using the ITask interface.
    /// </summary>
    public class BehaviorTreeRunner : TaskSystem
    {
        private Agent excecutingAgent;
        public ITask AbstractTree;
        private TaskState rootState;
        public BehaviorTreeAsset Tree;

        private void Awake()
        {
            excecutingAgent = GetComponent<Agent>();
            if (Tree != null)
            {
                AbstractTree = Tree.GetExecutableTree();
            }
        }

        public override void UpdateTaskSystem()
        {
            if (rootState != TaskState.Success && rootState != TaskState.Failure)
            {
                rootState = AbstractTree.Tick(excecutingAgent);
            }
        }
    }
}
