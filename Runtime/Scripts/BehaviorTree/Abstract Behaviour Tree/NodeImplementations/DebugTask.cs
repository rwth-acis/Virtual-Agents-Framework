using i5.VirtualAgents.TaskSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public class DebugTask : IAgentTask, ISerializable
    {
        public List<Func<bool>> ReadyToStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Func<bool>> ReadyToEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
    }
}
