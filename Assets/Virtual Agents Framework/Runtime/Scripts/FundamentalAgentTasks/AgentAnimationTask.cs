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
        private GameObject aimtarget;
        string layer;

        AimAtSomething aimScript;

        public AgentAnimationTask() { }

        public AgentAnimationTask(string startTrigger, float playTime, string stopTrigger = "", GameObject aimTarget = null, string layer = "")
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
            animator.SetTrigger(startTrigger);


            if (aimtarget != null)
            {
                if (layer == "")
                {
                    Debug.LogError("When aming at a target a layer coresponding to the body area that should aim at the target has to be choosen.");
                }


                aimScript = agent.gameObject.AddComponent<AimAtSomething>();

                //Not sure if this does anything, lllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
                agent.StartCoroutine(WaitForCurrentAnimationToFinish());
                aimScript.SetupAndStart(layer, aimtarget.transform);
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
        private IEnumerator WaitForCurrentAnimationToFinish()
        {
            yield return new WaitForSeconds(0.5f);
            while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
            {
                yield return null;
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