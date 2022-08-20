using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    /// <summary>
    /// Starts an animation by setting the trigger in the animator belonging to the agent. It will stop it after playtime second using the stop trigger if provided or else the startTrigger again
    /// </summary>
    public class AgentAnimationTask : AgentBaseTask, ISerializable
    {
        private Animator animator;
        private string startTrigger;
        private string stopTrigger;
        private float playTime;

        public AgentAnimationTask(){}

        public AgentAnimationTask(string startTrigger, float playTime, string stopTrigger = "")
        {
            this.startTrigger = startTrigger;
            this.stopTrigger = stopTrigger;
            this.playTime = playTime;
        }

        /// <summary>
        /// Starts the execution of the task; starts the animation
        /// </summary>
        /// <param name="agent">The agent on which the task is executed</param>
        public override void Execute(Agent agent)
        {
            animator = agent.GetComponent<Animator>();
            animator.SetTrigger(startTrigger);
            agent.StartCoroutine(Wait(playTime));
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public override void Stop()
        {
            animator.SetTrigger(stopTrigger != "" ? stopTrigger : startTrigger);
        }

        // wait for the given time and then finish the task
        private IEnumerator Wait(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            FinishTask();
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