using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using System;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes all its children one after another, but aborts if one child fails
    /// </summary>
    public class SequencerNode : ICompositeNode, ISerializable
    {
        public List<ITask> Children { get;  set; }
        public TaskState State { get; set; }

        private int current = 0;
        private Agent executingAgent;

        public SequencerNode()
        {
            Children = new List<ITask>();
        }

        public void Execute(Agent executingAgent)
        {
            this.executingAgent = executingAgent;
        }

        public void Stop()
        {
        }

        public TaskState Update()
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

        public void Serialize(TaskSerializer serializer)
        { }

        public void Deserialize(TaskSerializer serializer)
        { }
    }
}
