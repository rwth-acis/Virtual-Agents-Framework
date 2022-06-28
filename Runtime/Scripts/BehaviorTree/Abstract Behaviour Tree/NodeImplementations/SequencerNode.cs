using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using System;

namespace i5.VirtualAgents.BehaviourTrees
{
    public class SequencerNode : ICompositeNode, ISerializable
    {
        //TODO komplizierter getter setter vermutlich nicht nötig
        List<ITask> _children;
        public List<ITask> children { get { return _children; } set { _children = value; } }
        public TaskState rootState { get; set; }
        public List<Func<bool>> ReadyToStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Func<bool>> ReadyToEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        int current = 0;
        Agent executingAgent;

        public SequencerNode()
        {
            children = new List<ITask>();
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
            TaskState currentNodestate = children[current].FullUpdate(executingAgent);

            if (currentNodestate == TaskState.Success)
            {
                current++;
                if (current >= children.Count)
                    return TaskState.Success;
                else
                    return TaskState.Running;
            }
            else
            {
                return currentNodestate;
            }
        }

        public void Serialize(TaskSerializer serializer)
        { }

        public void Deserialize(TaskSerializer serializer)
        { }
    }
}
