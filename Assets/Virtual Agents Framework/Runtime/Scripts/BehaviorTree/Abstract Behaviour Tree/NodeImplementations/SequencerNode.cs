using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.AgentTasks;
using System;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes all its children one after another, but aborts if one child fails
    /// </summary>
    public class SequencerNode : CompositeNode, ISerializable
    {
        private int current;

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            current = 0;
        }

        public override TaskState Update()
        {
            TaskState currentNodestate = Children[current].FullUpdate(executingAgent);

            if (currentNodestate == TaskState.Success)
            {
                current++;
                if (current >= Children.Count)
                    return TaskState.Success;
                else
                    return TaskState.Running;
            }
            else
            {
                // This lets this node automatically fail once the first child fails
                return currentNodestate;
            }
        }

        public void Serialize(SerializationDataContainer serializer)
        { }

        public void Deserialize(SerializationDataContainer serializer)
        { }
    }
}
