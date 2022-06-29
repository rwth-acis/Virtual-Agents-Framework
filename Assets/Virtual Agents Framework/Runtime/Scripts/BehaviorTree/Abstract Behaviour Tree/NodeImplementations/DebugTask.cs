using i5.VirtualAgents.TaskSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    public class DebugTask : AgentBaseTask, ISerializable
    {

        public override TaskState Update()
        {
            Debug.Log("Debug Task ececuted");
            return TaskState.Success;
        }

        public void Serialize(TaskSerializer serializer)
        {
        }

        public void Deserialize(TaskSerializer serializer)
        {
        }
    }
}
