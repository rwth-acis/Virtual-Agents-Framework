using i5.VirtualAgents.ScheduleBasedExecution;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    // Displays a message in the debug log on execution
    public class DebugTask : AgentBaseTask, ISerializable
    {

        public override void StartExecution(Agent agent)
        {
            Debug.Log("Debug task executed");
            FinishTask();
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
        }
    }
}
