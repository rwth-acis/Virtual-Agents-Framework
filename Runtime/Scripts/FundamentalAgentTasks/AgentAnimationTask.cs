using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
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
        private readonly GameObject aimTarget;
        private readonly string layer;

        AimAt aimScript;

        AdaptiveGaze lookAroundController;
        public AgentAnimationTask() { }

        public AgentAnimationTask(string startTrigger, float playTime, string stopTrigger = "", string layer = "", GameObject aimTarget = null)
        {
            this.startTrigger = startTrigger;
            this.stopTrigger = stopTrigger;
            this.playTime = playTime;
            this.aimTarget = aimTarget;
            this.layer = layer;
        }

        /// <summary>
        /// Starts the execution of the task; starts the animation
        /// </summary>
        /// <param name="agent">The agent on which the task is executed</param>
        public override void StartExecution(Agent agent)
        {
            animator = agent.GetComponent<Animator>();
            lookAroundController = agent.GetComponent<AdaptiveGaze>();
            animator.SetTrigger(startTrigger);


            if (aimTarget != null)
            {
                if (layer == "")
                {
                    Debug.LogError("When aming at a target a layer coresponding to the body area that should aim at the target has to be choosen.");
                }


                aimScript = agent.gameObject.AddComponent<AimAt>();

                agent.StartCoroutine(WaitForCurrentAnimationToFinishAndStartAimScript());

            }
            agent.StartCoroutine(Wait(playTime));
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public override void StopExecution()
        {
            if (aimTarget != null)
            {
                aimScript.Stop();
                if (lookAroundController != null && layer == "Head")
                {
                    lookAroundController.Activate();
                }
            }
            animator.SetTrigger(stopTrigger != "" ? stopTrigger : startTrigger);
        }

        // wait for the given time and then finish the task
        private IEnumerator Wait(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            FinishTask();
        }

        // wait for current animation to finish
        private IEnumerator WaitForCurrentAnimationToFinishAndStartAimScript()
        {
            // TODO: this value is currently hardcoded. We should look for a solution where the animation is played and we are notified once it is done.
            // Pointing animation takes 26 frames to finish
            yield return new WaitForSeconds(0.5f);
            aimScript.SetupAndStart(layer, aimTarget.transform);
            // If the agent is setup to look around, stop it while aiming with the head
            if (lookAroundController != null && layer == "Head")
            {
                lookAroundController.Deactivate();
            }
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Start Trigger", startTrigger);
            serializer.AddSerializedData("Stop Trigger", stopTrigger);
            serializer.AddSerializedData("Play Time", playTime);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            startTrigger = serializer.GetSerializedString("Start Trigger");
            stopTrigger = serializer.GetSerializedString("Stop Trigger");
            playTime = serializer.GetSerializedFloat("Play Time");
        }

    }
}