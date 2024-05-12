using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.ScheduleBasedExecution;
using UnityEditor;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Allows a task be be serialized. Neccessary in order to use them with the visual Behaviour Tree editor.
    /// </summary>
    public interface ISerializable
    {
        void Serialize(SerializationDataContainer serializer);

        void Deserialize(SerializationDataContainer serializer);
    }
}
