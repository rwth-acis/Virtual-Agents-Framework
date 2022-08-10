using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents.BehaviourTrees.Visual
{
    public class RootNode : DecoratorNode, IRootNode, ISerializable
    {
        public void Deserialize(SerializationDataContainer serializer)
        {
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }
    }
}
