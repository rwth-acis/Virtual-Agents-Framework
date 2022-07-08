using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using UnityEditor;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Allows a task be be serialized. Neccesarry in order to use them with the visual Behaviour Tree editor.
    /// </summary>
    public interface ISerializable
    {
        void Serialize(TaskSerializer serializer);

        void Deserialize(TaskSerializer serializer);
    }
}
