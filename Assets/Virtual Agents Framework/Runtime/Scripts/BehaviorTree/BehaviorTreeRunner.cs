using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.BehaviourTrees.Visual;
using System;

namespace i5.VirtualAgents.BehaviourTrees
{
    [Serializable]
    public class NodesOverwriteData : SerializationData<SerializationDataContainer> { }

    /// <summary>
    /// Executes a given behaviour tree until the root node reports sucess or failure.
    /// </summary>
    public class BehaviorTreeRunner : MonoBehaviour, ITaskSystem
    {
        public Agent excecutingAgent;
        public BehaviorTreeAsset behaviourTree;
        public ITask abstractTree;
        private TaskState rootState;

        public NodesOverwriteData nodesOverwriteData = new NodesOverwriteData();

        public void OnEnable()
        {
            abstractTree = behaviourTree.GetExecutableTree(nodesOverwriteData);
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
