using i5.VirtualAgents.TaskSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    public class DebugTask : IAgentTask, ISerializable
    {
        public List<Func<bool>> ReadyToStart { get; set; }
        public List<Func<bool>> ReadyToEnd { get; set; }
        public TaskState rootState { get; set; }



        public void Execute(Agent exceutingAgent)
        {
        }

 

        public void Stop()
        {
        }

        TaskState ITask.Update()
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
