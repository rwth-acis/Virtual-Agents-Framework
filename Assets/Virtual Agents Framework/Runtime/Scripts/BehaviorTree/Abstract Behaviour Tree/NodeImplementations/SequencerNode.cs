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
    public class SequencerNode : BaseTask, ICompositeNode, ISerializable
    {
        public List<ITask> Children { get;  set; }

        private int current = 0;
        private Agent executingAgent;

        public SequencerNode()
        {
            Children = new List<ITask>();
        }

        public override void StartExecution(Agent executingAgent)
        {
            this.executingAgent = executingAgent;
        }


        public override TaskState EvaluateTaskState()
        {
            TaskState currentNodestate = Children[current].Tick(executingAgent);

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
