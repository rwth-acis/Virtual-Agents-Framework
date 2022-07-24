using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using System;
using UnityEditor;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes its child one after another, until one succsedes
    /// </summary>
    public class SelectorNode : BaseTask, ICompositeNode, ISerializable
    {
        public List<ITask> Children { get; set; }

        private Agent executingAgent;
        public SelectorNode()
        {
            Children = new List<ITask>();
        }

        
        public override void Execute(Agent executingAgent)
        {
            this.executingAgent = executingAgent;
        }
        int current = 0;

        public override TaskState Update()
        {
            TaskState currentNodestate = Children[current].FullUpdate(executingAgent);


            if (currentNodestate == TaskState.Failure)
            {
                current++;
                if (current >= Children.Count)
                    return TaskState.Failure; //All nodes failed, report general failure
                else
                    return TaskState.Running; 
            }
            else
            {
                return currentNodestate;
            }
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
        }
    }
}
