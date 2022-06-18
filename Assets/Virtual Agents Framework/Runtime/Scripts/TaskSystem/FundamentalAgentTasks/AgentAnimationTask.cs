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
        private DateTime startTime;

        public AgentAnimationTask(string startTrigger, float playTime, string stopTrigger = "")
        {
            this.startTrigger = startTrigger;
            this.stopTrigger = stopTrigger;
            this.playTime = playTime;
        }

        public override void Execute(Agent agent)
        {
            base.Execute(agent);
            animator = agent.GetComponent<Animator>();
            animator.SetTrigger(startTrigger);
            startTime = DateTime.Now;
        }

        public override void Update()
        {
            base.Update();
            if ((DateTime.Now - startTime).Seconds > playTime)
            {
                animator.SetTrigger(stopTrigger != "" ? stopTrigger : startTrigger);
                FinishTask();
            }
        }
    }
}
