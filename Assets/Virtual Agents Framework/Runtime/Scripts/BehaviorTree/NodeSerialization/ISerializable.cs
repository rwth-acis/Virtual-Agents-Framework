using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents
{
    public interface ISerializable
    {
        void Serialize(TaskSerializer serializer) { }

        void Deserialize(TaskSerializer serializer) { }
    }
}
