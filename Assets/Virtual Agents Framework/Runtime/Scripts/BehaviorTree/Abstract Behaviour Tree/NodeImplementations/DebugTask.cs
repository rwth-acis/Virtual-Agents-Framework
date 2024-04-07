using i5.VirtualAgents.ScheduleBasedExecution;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// A task which logs a message to the debug console on execution
    /// </summary>
    public class DebugTask : AgentBaseTask, ISerializable
    {
        public string message = "Debug task executed";

        public override void StartExecution(Agent agent)
        {
            Debug.Log(message);
            FinishTask();
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("message", message);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            message = serializer.GetSerializedString("message");
        }
    }
}
