using System;
using System.Collections;
using System.Collections.Generic;
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

        public AgentAnimationTask(){}

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
            

            if(aimtarget != null)
            {
                if(layer == "")
                {
                    Debug.LogError("When aming at a target a layer coresponding to the body area that should aim at the target has to be choosen.");
                }


                aimScript = agent.gameObject.AddComponent<AimAtSomething>();
                aimScript.SetTargetTransform(aimtarget.transform);

                Transform raycast = null;
                switch (layer)
                {
                    case "Right Arm":
                        raycast = agent.transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R/Palm1.R/IndexProximal.R/IndexIntermediate.R/IndexDistal.R/IndexDistal.R_end");
                        break;
                    case "Left Arm":
                        raycast = agent.transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/Palm1.L/IndexProximal.L/IndexIntermediate.L/IndexDistal.L/IndexDistal.L_end");
                        break;
                    case "Right Leg":
                        raycast = agent.transform.Find("MeshDeformRig/Hips/UpperLeg.R/LowerLeg.R/Foot.R/Toes.R/Toes.R_end");
                        break;
                    case "Left Leg":
                        raycast = agent.transform.Find("MeshDeformRig/Hips/UpperLeg.L/LowerLeg.L/Foot.L/Toes.L/Toes.L_end");
                        break;
                    case "Head":
                        raycast = agent.transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Neck/Head");
                        break;
                    case "Base Layer":
                        raycast = agent.transform.Find("MeshDeformRig/Hips/Spine/Chest");
                        break;
                    default:
                        Debug.LogWarning("Animation layer is not specified or could not be assosiatet to a body part that should aim at the target. Using UpperChest instead for layer named:" + layer);
                        raycast = agent.transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest");
                        break;
                }
                if(raycast == null)
                {
                    Debug.LogError("Body part that should be used for aiming could not be found on the agent");
                }
                agent.StartCoroutine(WaitForCurrentAnimationToFinishAnThenStartAiming(aimScript, raycast));
                aimScript.setBonePreset(layer);
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
                aimScript.StartCoroutine(aimScript.fadeStop());
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
        private IEnumerator WaitForCurrentAnimationToFinishAnThenStartAiming(AimAtSomething aimScript, Transform raycast)
        {
            yield return new WaitForSeconds(0.5f);
            while(animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f) { 
                yield return null;
            }
            aimScript.SetAimTransform(raycast);
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