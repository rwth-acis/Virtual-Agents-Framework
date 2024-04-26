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

                switch (layer)
                {
                    case "Right Arm":
                        aimScript = agent.gameObject.AddComponent<RightArmPreset>();
                        break;
                    case "Left Arm":
                        aimScript = agent.gameObject.AddComponent<LeftArmPreset>();
                        break;
                    case "Right Leg":
                        aimScript = agent.gameObject.AddComponent<RightLegPreset>();
                        break;
                    case "Left Leg":
                        aimScript = agent.gameObject.AddComponent<LeftLegPreset>();
                        break;
                    case "Head":
                        aimScript = agent.gameObject.AddComponent<HeadPreset>();
                        break;
                    case "Base Layer":
                        aimScript = agent.gameObject.AddComponent<BaseLayerPreset>();
                        break;
                    default:
                        Debug.LogWarning("No boneset avaiable for the layer named:" + layer);
                        break;
                }

                

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

        // wait for current animation to finish and start the aim script if the agent is supposed to aim at a target
        private IEnumerator WaitForCurrentAnimationToFinishAndStartAimScript()
        {
            int layerIndex = animator.GetLayerIndex(layer);
            if (layerIndex == -1)
            {
                Debug.LogError("The layer " + layer + " does not exist in the animator of the agent.");
            }
            // normalizedTims goes from X.0 to X+1 for each animation cycle, so we wait until the next animation cycle starts or the animation cycle with a diffrent animation begins
            int endOfNextAnimation = (int)System.Math.Ceiling(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime);
            while (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < endOfNextAnimation && !(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < (endOfNextAnimation - 1)))
            {
                yield return null;
            }

            aimScript.SetupAndStart(aimTarget.transform);
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