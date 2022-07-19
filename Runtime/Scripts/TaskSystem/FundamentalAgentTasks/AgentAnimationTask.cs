using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    /// <summary>
    /// Starts an animation by setting the trigger in the animator belonging to the agent. It will stop it after playtime second using the stop trigger if provided or else the startTrigger again
    /// </summary>
    public class AgentAnimationTask : AgentBaseTask
    {
        private Animator animator;
        private string startTrigger;
        private string stopTrigger;
        private float playTime;
        private float startTime;

        /// <summary>
        /// Creates the animation task instance
        /// </summary>
        /// <param name="startTrigger">The name of the trigger in the animator which starts the animation</param>
        /// <param name="playTime">How many seconds the animation should be played</param>
        /// <param name="stopTrigger">The name of the trigger in the animator which stops the animation; if it is not set, the start trigger will be called again to toggle the animation</param>
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
            base.Execute(agent);
            animator = agent.GetComponent<Animator>();
            animator.SetTrigger(startTrigger);
            startTime = Time.time;
        }

        /// <summary>
        /// Called every frame as long as the task is active
        /// Makes sure that the animation is played for the configured time frame
        /// </summary>
        public override void Update()
        {
            base.Update();
            if ((Time.time - startTime) > playTime)
            {
                animator.SetTrigger(stopTrigger != "" ? stopTrigger : startTrigger);
                FinishTask();
            }
        }
    }
}
