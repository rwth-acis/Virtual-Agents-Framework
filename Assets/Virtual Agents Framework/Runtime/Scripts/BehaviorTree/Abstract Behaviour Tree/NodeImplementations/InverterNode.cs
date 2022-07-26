using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents.BehaviourTrees
{
    public class InverterNode : DecoratorNode, ISerializable
    {

        public override TaskState Update()
        {
            TaskState state = Child.FullUpdate(executingAgent);
            if (state == TaskState.Running || state == TaskState.Waiting)
            {
                return state;
            }
            if (state == TaskState.Success)
            {
                return TaskState.Failure;
            }
            if (state == TaskState.Failure)
            {
                return TaskState.Success;
            }
            throw new NotImplementedException();
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }

    }
}
