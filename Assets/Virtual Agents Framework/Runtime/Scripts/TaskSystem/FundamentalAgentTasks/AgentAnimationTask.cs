using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    /// <summary>
    /// Starts an animation by setting the trigger in the animator belonging to the agent. It will stop it after playtime second using the stop trigger if provided or else the startTrigger again
    /// </summary>
    public class AgentAnimationTask : IAgentTask, ISerializable
    {
        /// <summary>
        /// Event which is invoked once the task is finished
        /// </summary>
        public event Action OnTaskFinished;

        public List<Func<bool>> ReadyToStart { get; set; }
        public List<Func<bool>> ReadyToEnd { get; set; }
        public TaskState rootState { get; set; }

        private Animator animator;
        private string startTrigger;
        private string stopTrigger;
        private float playTime;
        private DateTime startTime;

        public AgentAnimationTask(){}

        public AgentAnimationTask(string startTrigger, float playTime, string stopTrigger = "")
        {
            this.startTrigger = startTrigger;
            this.stopTrigger = stopTrigger;
            this.playTime = playTime;
        }

        public void Execute(Agent agent)
        {
            animator = agent.GetComponent<Animator>();
            animator.SetTrigger(startTrigger);
            startTime = DateTime.Now;
        }

        public TaskState Update()
        {
            if ((DateTime.Now - startTime).Seconds > playTime)
            {
                return TaskState.Success;
            }
            return TaskState.Running;
        }

        public void Stop()
        {
            animator.SetTrigger(stopTrigger != "" ? stopTrigger : startTrigger);
        }

        public void Serialize(TaskSerializer serializer)
        {
            serializer.AddSerializedData("Start Trigger", startTrigger);
            serializer.AddSerializedData("Stop Trigger", stopTrigger);
            serializer.AddSerializedData("Play Time", playTime);
        }

        public void Deserialize(TaskSerializer serializer)
        {
            startTrigger = serializer.GetSerializedString("Start Trigger");
            stopTrigger = serializer.GetSerializedString("Stop Trigger");
            playTime = serializer.GetSerializedFloat("Play Time");
        }

    }
}
