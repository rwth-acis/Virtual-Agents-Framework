using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using UnityEditor;

namespace i5.VirtualAgents.TaskSystem
{
    public interface ISerializable
    {
        void Serialize(TaskSerializer serializer) { }

        void Deserialize(TaskSerializer serializer) { }
    }
}
