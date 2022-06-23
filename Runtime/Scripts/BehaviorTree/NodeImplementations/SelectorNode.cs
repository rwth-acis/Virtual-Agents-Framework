using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using System;
using UnityEditor;

namespace i5.VirtualAgents
{
    [Serializable]
    public class SelectorNode : ICompositeNode, ISerializable
    {
        public List<ITask> children { get; set; }
        public TaskState rootState { get; set; }

        private Agent executingAgent;
        public SelectorNode()
        {
            children = new List<ITask>();
        }

        
        public void Execute(Agent executingAgent)
        {
            rootState = TaskState.Running;
            this.executingAgent = executingAgent;
        }
        int current = 0;

        public void Stop()
        {
        }

        public TaskState Update()
        {
            TaskState currentNodestate = children[current].FullUpdate(executingAgent);


            if (currentNodestate == TaskState.Failure)
            {
                current++;
                if (current >= children.Count)
                    return TaskState.Failure; //All nodes failed, report general failure
                else
                    return TaskState.Running; 
            }
            else
            {
                return currentNodestate;
            }
        }
    }
}
