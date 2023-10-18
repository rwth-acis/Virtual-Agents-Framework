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
        private readonly GameObject aimtarget;
        private readonly string layer;

        AimAtSomething aimScript;

        LookAround lookAroundController;
        public AgentAnimationTask() { }

        public AgentAnimationTask(string startTrigger, float playTime, string stopTrigger = "", string layer = "", GameObject aimTarget = null)
        {
            this.startTrigger = startTrigger;
            this.stopTrigger = stopTrigger;
            this.playTime = playTime;
            this.aimtarget = aimTarget;
            this.layer = layer;
        }

        /// <summary>
        /// Starts the execution of the task; starts the animation
        /// </summary>
        /// <param name="agent">The agent on which the task is executed</param>
        public override void StartExecution(Agent agent)
        {
            animator = agent.GetComponent<Animator>();
            lookAroundController = agent.GetComponent<LookAround>();
            animator.SetTrigger(startTrigger);


            if (aimtarget != null)
            {
                if (layer == "")
                {
                    Debug.LogError("When aming at a target a layer coresponding to the body area that should aim at the target has to be choosen.");
                }


                aimScript = agent.gameObject.AddComponent<AimAtSomething>();
                Debug.Log("Aimscript added to agent");

                agent.StartCoroutine(WaitForCurrentAnimationToFinishAndStartAimScript());

            }
            agent.StartCoroutine(Wait(playTime));
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public override void StopExecution()
        {
            if (aimtarget != null)
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
            //Pointat animation takes 26 frames to finish
            yield return new WaitForSeconds(0.5f);
            aimScript.SetupAndStart(layer, aimtarget.transform);
            //If the agent is setup to look around, stop it while aiming with the head
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