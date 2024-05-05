using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.BehaviourTrees.Visual;
using System;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    [Serializable]
    public class NodesOverwriteData : SerializationData<SerializationDataContainer> { }

    /// <summary>
    /// Executes a given behaviour tree until the root node reports sucess or failure. Can either be provided with a Behaviour Tree Asset or can be given an AbstractTree manually constructed using the ITask interface.
    /// </summary>
    public class BehaviorTreeRunner : TaskSystem
    {
        private Agent executingAgent;
        public ITask AbstractTree;
        private TaskState rootState;
        public BehaviorTreeAsset Tree;

        public NodesOverwriteData nodesOverwriteData = new NodesOverwriteData();
        private void Awake()
        {
            executingAgent = GetComponent<Agent>();
            if (Tree != null)
            {
                AbstractTree = Tree.GetExecutableTree(nodesOverwriteData);
            }
        }

        public override void UpdateTaskSystem()
        {
            if (rootState != TaskState.Success && rootState != TaskState.Failure)
            {
                rootState = AbstractTree.Tick(executingAgent);
            }
        }
    }
}
