using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Enables an agent to interact with objects in its proximity in the background.
    /// Supports custom animations via AnimatorOverrideController and IK targeting (Reach or LookAt).
    /// </summary>
    public class AgentInteractionTask : AgentBaseTask, ISerializable
    {
        public Interactable AssociatedInteractable { get; protected set; }
        public AnimationClip InteractionClip { get; protected set; }
        public GameObject TargetObject { get; protected set; }
        public string LayerName { get; protected set; }
        public string ClipToOverride { get; protected set; }
        public string StateName { get; protected set; }
        public InteractionIKMode IkMode { get; protected set; }
        public float Duration { get; protected set; }
        public float AnimationSpeed { get; protected set; }
        public float MaxWeight { get; protected set; } = 1.0f;
        public Transform TouchBone { get; protected set; }
        public HumanBodyBones TouchHumanBone { get; protected set; } = HumanBodyBones.LastBone;
        public string TouchBoneName { get; protected set; }

        private bool finished = false;
        private bool contactTriggered = false;
        private Animator animator;
        private AnimatorOverrideController overrideController;
        private RuntimeAnimatorController originalController;
        private MonoBehaviour activeAimScript;
        private TwoBoneIKConstraint armConstraint;
        private Vector3 originalIKTargetPosition;
        private Quaternion originalIKTargetRotation;
        private Transform originalTip;
        private bool tipModified = false;
        private Coroutine ikReachCoroutine;
        private Coroutine contactCoroutine;
        private Coroutine timerCoroutine;
        private Coroutine activeAimCoroutine;

        private string GetPlaceholderForLayer(string layer)
        {
            return layer switch
            {
                "Left Arm" => "LeftPlaceholder",
                "Right Arm" => "RightPlaceholder",
                "Head" => "HeadPlaceholder",
                "Right Leg" => "RightLegPlaceholder",
                "Left Leg" => "LeftLegPlaceholder",
                _ => "BasePlaceholder"
            };
        }

        public AgentInteractionTask() { }

        /// <summary>
        /// Creates an AgentInteractionTask based on an Interactable component.
        /// </summary>
        /// <param name="interactable">The interactable object in the scene.</param>
        public AgentInteractionTask(Interactable interactable)
        {
            AssociatedInteractable = interactable;
            InteractionClip = interactable.InteractionClip;
            TargetObject = interactable.InteractionTarget != null ? interactable.InteractionTarget.gameObject : null;
            LayerName = interactable.LayerName;
            ClipToOverride = GetPlaceholderForLayer(LayerName);
            StateName = GetPlaceholderForLayer(LayerName);
            IkMode = interactable.IkMode;
            Duration = interactable.Duration;
            AnimationSpeed = interactable.AnimationSpeed;
            MaxWeight = interactable.MaxWeight;
            TouchBone = interactable.TouchBone;
            TouchHumanBone = interactable.TouchHumanBone;
            TouchBoneName = interactable.TouchBoneName;
        }

        /// <summary>
        /// Creates a custom AgentInteractionTask with explicit parameters.
        /// </summary>
        public AgentInteractionTask(
            AnimationClip interactionClip,
            GameObject targetObject = null,
            InteractionIKMode ikMode = InteractionIKMode.None,
            string layerName = "Right Arm",
            string clipToOverride = "",
            string stateName = "",
            float duration = 2.0f,
            float animationSpeed = 2.0f,
            float maxWeight = 1.0f,
            Transform touchBone = null,
            HumanBodyBones touchHumanBone = HumanBodyBones.LastBone,
            string touchBoneName = ""
        )
        {
            InteractionClip = interactionClip;
            TargetObject = targetObject;
            IkMode = ikMode;
            LayerName = layerName;
            ClipToOverride = string.IsNullOrEmpty(clipToOverride) ? GetPlaceholderForLayer(layerName) : clipToOverride;
            StateName = string.IsNullOrEmpty(stateName) ? GetPlaceholderForLayer(layerName) : stateName;
            Duration = duration;
            AnimationSpeed = animationSpeed;
            MaxWeight = Mathf.Clamp01(maxWeight);
            TouchBone = touchBone;
            TouchHumanBone = touchHumanBone;
            TouchBoneName = touchBoneName;
        }

        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);

            animator = agent.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Agent has no Animator component.");
                FinishTaskAsFailed();
                return;
            }

            finished = false;
            contactTriggered = false;

            // 1. Trigger Started Event
            if (AssociatedInteractable != null)
            {
                AssociatedInteractable.OnInteractionStarted?.Invoke();
            }

            // 2. Setup Animation Override
            if (InteractionClip != null)
            {
                originalController = animator.runtimeAnimatorController;

                // Ensure we don't double-wrap overrides
                if (originalController is AnimatorOverrideController existingOverride)
                {
                    overrideController = existingOverride;
                }
                else
                {
                    overrideController = new AnimatorOverrideController(originalController);
                    animator.runtimeAnimatorController = overrideController;
                }

                if (string.IsNullOrEmpty(ClipToOverride))
                {
                    ClipToOverride = GetPlaceholderForLayer(LayerName);
                }
                if (string.IsNullOrEmpty(StateName))
                {
                    StateName = GetPlaceholderForLayer(LayerName);
                }

                overrideController[ClipToOverride] = InteractionClip;

                int layerIndex = animator.GetLayerIndex(LayerName);
                if (layerIndex != -1)
                {
                    animator.Play(StateName, layerIndex, 0f);
                }
                else
                {
                    Debug.LogWarning($"Animator layer '{LayerName}' not found on agent.");
                }
            }

            // 3. Setup IK Targeting
            if (TargetObject != null)
            {
                if (IkMode == InteractionIKMode.Reach)
                {
                    MeshSockets meshSockets = agent.GetComponent<MeshSockets>();
                    if (meshSockets != null)
                    {
                        if (LayerName == "Left Arm")
                        {
                            armConstraint = meshSockets.TwoBoneIKConstraintLeftArm;
                        }
                        else
                        {
                            armConstraint = meshSockets.TwoBoneIKConstraintRightArm;
                        }

                        if (armConstraint != null)
                        {
                            // Resolve Touch Bone if configured
                            Transform resolvedTouchBone = TouchBone;
                            if (resolvedTouchBone == null && TouchHumanBone != HumanBodyBones.LastBone)
                            {
                                if (animator != null && animator.isHuman)
                                {
                                    resolvedTouchBone = animator.GetBoneTransform(TouchHumanBone);
                                }
                            }
                            if (resolvedTouchBone == null && !string.IsNullOrEmpty(TouchBoneName))
                            {
                                Transform[] allTransforms = agent.GetComponentsInChildren<Transform>();
                                foreach (Transform t in allTransforms)
                                {
                                    if (t.name.Equals(TouchBoneName, System.StringComparison.OrdinalIgnoreCase))
                                    {
                                        resolvedTouchBone = t;
                                        break;
                                    }
                                }
                            }

                            // If touch bone is assigned to another instance or hierarchy, find corresponding bone on this agent
                            if (resolvedTouchBone != null && !resolvedTouchBone.IsChildOf(agent.transform))
                            {
                                Transform[] allTransforms = agent.GetComponentsInChildren<Transform>();
                                foreach (Transform t in allTransforms)
                                {
                                    if (t.name.Equals(resolvedTouchBone.name, System.StringComparison.OrdinalIgnoreCase))
                                    {
                                        resolvedTouchBone = t;
                                        break;
                                    }
                                }
                            }

                            originalTip = armConstraint.data.tip;
                            if (resolvedTouchBone != null && resolvedTouchBone != originalTip)
                            {
                                armConstraint.data.tip = resolvedTouchBone;
                                tipModified = true;
                                RigBuilder rigBuilder = agent.GetComponent<RigBuilder>();
                                if (rigBuilder != null)
                                {
                                    rigBuilder.Build();
                                }
                            }

                            if (armConstraint.data.target != null)
                            {
                                originalIKTargetPosition = armConstraint.data.target.position;
                                originalIKTargetRotation = armConstraint.data.target.rotation;
                            }
                            ikReachCoroutine = agent.StartCoroutine(RunIKReachCoroutine(agent));
                        }
                        else
                        {
                            Debug.LogWarning("Selected arm constraint is null on MeshSockets.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Agent lacks MeshSockets component required for Reach IK.");
                    }
                }
                else if (IkMode == InteractionIKMode.LookAt)
                {
                    switch (LayerName)
                    {
                        case "Right Arm":
                            activeAimScript = agent.gameObject.AddComponent<RightArmPreset>();
                            break;
                        case "Left Arm":
                            activeAimScript = agent.gameObject.AddComponent<LeftArmPreset>();
                            break;
                        case "Right Leg":
                            activeAimScript = agent.gameObject.AddComponent<RightLegPreset>();
                            break;
                        case "Left Leg":
                            activeAimScript = agent.gameObject.AddComponent<LeftLegPreset>();
                            break;
                        case "Head":
                            activeAimScript = agent.gameObject.AddComponent<HeadPreset>();
                            break;
                        case "Base Layer":
                            activeAimScript = agent.gameObject.AddComponent<BaseLayerPreset>();
                            break;
                        default:
                            Debug.LogWarning("No bone set preset available for the layer named: " + LayerName);
                            break;
                    }

                    if (activeAimScript != null)
                    {
                        activeAimCoroutine = agent.StartCoroutine(WaitForCurrentAnimationToFinishAndStartAimScript(agent));
                    }
                }
            }

            // Start Contact Timer for non-Reach mode
            if (AssociatedInteractable != null && (IkMode != InteractionIKMode.Reach || TargetObject == null))
            {
                contactCoroutine = agent.StartCoroutine(RunNonReachContactCoroutine());
            }

            // 4. Start Duration Timer
            timerCoroutine = agent.StartCoroutine(TimerCoroutine());
        }

        private IEnumerator RunIKReachCoroutine(Agent agent)
        {
            float transitionDuration = Mathf.Min(1.0f / AnimationSpeed, Duration / 2f);
            float holdDuration = Duration - (2f * transitionDuration);
            float time = 0f;

            if (armConstraint == null || armConstraint.data.target == null || armConstraint.data.tip == null)
            {
                yield break;
            }

            Vector3 startPos = armConstraint.data.tip.position;
            Quaternion startRot = armConstraint.data.tip.rotation;

            // Phase 1: Reach Out (Fade In)
            while (time < transitionDuration)
            {
                time += Time.deltaTime;
                float progress = time / transitionDuration;

                armConstraint.weight = progress * MaxWeight;
                armConstraint.data.target.position = Vector3.Lerp(startPos, TargetObject.transform.position, progress);
                armConstraint.data.target.rotation = Quaternion.Lerp(startRot, TargetObject.transform.rotation, progress);

                yield return null;
            }

            // Trigger Contact Event when peak reach is made
            if (AssociatedInteractable != null && !contactTriggered)
            {
                contactTriggered = true;
                AssociatedInteractable.OnInteractionContact?.Invoke();
            }

            // Phase 2: Hold (Interact)
            time = 0f;
            while (time < holdDuration)
            {
                time += Time.deltaTime;
                armConstraint.weight = MaxWeight;
                armConstraint.data.target.position = TargetObject.transform.position;
                armConstraint.data.target.rotation = TargetObject.transform.rotation;
                yield return null;
            }

            // Phase 3: Retract (Fade Out)
            time = 0f;
            while (time < transitionDuration)
            {
                time += Time.deltaTime;
                float progress = time / transitionDuration;

                armConstraint.weight = (1f - progress) * MaxWeight;
                armConstraint.data.target.position = Vector3.Lerp(TargetObject.transform.position, originalIKTargetPosition, progress);
                armConstraint.data.target.rotation = Quaternion.Lerp(TargetObject.transform.rotation, originalIKTargetRotation, progress);
                yield return null;
            }

            armConstraint.weight = 0f;
            armConstraint.data.target.position = originalIKTargetPosition;
            armConstraint.data.target.rotation = originalIKTargetRotation;
        }

        private IEnumerator RunNonReachContactCoroutine()
        {
            float transitionDuration = Mathf.Min(1.0f / AnimationSpeed, Duration / 2f);
            yield return new WaitForSeconds(transitionDuration);
            if (AssociatedInteractable != null && !contactTriggered)
            {
                contactTriggered = true;
                AssociatedInteractable.OnInteractionContact?.Invoke();
            }
        }

        private IEnumerator TimerCoroutine()
        {
            yield return new WaitForSeconds(Duration);
            finished = true;
            FinishTask();
        }

        private IEnumerator WaitForCurrentAnimationToFinishAndStartAimScript(Agent agent)
        {
            int layerIndex = animator.GetLayerIndex(LayerName);
            if (layerIndex == -1)
            {
                Debug.LogError("The layer " + LayerName + " does not exist in the animator of the agent.");
                yield break;
            }

            int endOfNextAnimation = (int)System.Math.Ceiling(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime);
            while (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < endOfNextAnimation && 
                   !(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < (endOfNextAnimation - 1)))
            {
                yield return null;
            }

            if (activeAimScript != null)
            {
                AimAt aimAt = activeAimScript as AimAt;
                if (aimAt != null)
                {
                    aimAt.SetupAndStart(TargetObject.transform);
                }
            }

            AdaptiveGaze lookAroundController = agent.GetComponent<AdaptiveGaze>();
            if (lookAroundController != null && LayerName == "Head")
            {
                lookAroundController.Deactivate();
            }
        }

        public override TaskState EvaluateTaskState()
        {
            if (finished)
            {
                return TaskState.Success;
            }
            return TaskState.Running;
        }

        public override void StopExecution()
        {
            base.StopExecution();

            // 1. Trigger Ended Event
            if (AssociatedInteractable != null)
            {
                AssociatedInteractable.OnInteractionEnded?.Invoke();
            }

            // 2. Clean up Coroutines
            if (executingAgent != null)
            {
                if (ikReachCoroutine != null)
                {
                    executingAgent.StopCoroutine(ikReachCoroutine);
                }
                if (contactCoroutine != null)
                {
                    executingAgent.StopCoroutine(contactCoroutine);
                }
                if (timerCoroutine != null)
                {
                    executingAgent.StopCoroutine(timerCoroutine);
                }
                if (activeAimCoroutine != null)
                {
                    executingAgent.StopCoroutine(activeAimCoroutine);
                }
            }

            // 3. Clean up AimAt scripts
            if (activeAimScript != null)
            {
                AimAt aimAt = activeAimScript as AimAt;
                if (aimAt != null)
                {
                    aimAt.Stop();
                }
                Object.Destroy(activeAimScript);

                AdaptiveGaze lookAroundController = executingAgent != null ? executingAgent.GetComponent<AdaptiveGaze>() : null;
                if (lookAroundController != null && LayerName == "Head")
                {
                    lookAroundController.Activate();
                }
            }

            // 4. Reset IK Constraint & Revert temporary touch bone tip
            if (armConstraint != null)
            {
                armConstraint.weight = 0f;
                if (armConstraint.data.target != null)
                {
                    armConstraint.data.target.position = originalIKTargetPosition;
                    armConstraint.data.target.rotation = originalIKTargetRotation;
                }

                if (tipModified && originalTip != null)
                {
                    armConstraint.data.tip = originalTip;
                    tipModified = false;
                    RigBuilder rigBuilder = executingAgent != null ? executingAgent.GetComponent<RigBuilder>() : null;
                    if (rigBuilder != null)
                    {
                        rigBuilder.Build();
                    }
                }
            }

            // 5. Crossfade to Idle on this layer
            if (animator != null)
            {
                int layerIndex = animator.GetLayerIndex(LayerName);
                if (layerIndex != -1)
                {
                    // Idle state is either called "Idle" or "Idle | Walk" depending on the layer
                    if (animator.HasState(layerIndex, Animator.StringToHash("Idle")))
                    {
                        animator.CrossFade("Idle", 0.25f, layerIndex);
                    }
                    else if (animator.HasState(layerIndex, Animator.StringToHash("Idle | Walk")))
                    {
                        animator.CrossFade("Idle | Walk", 0.25f, layerIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"No Idle state found in layer '{LayerName}' of the animator. Could not exit interaction animation.");
                    }
                }
            }
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Layer Name", LayerName);
            serializer.AddSerializedData("Clip to Override", ClipToOverride);
            serializer.AddSerializedData("State Name", StateName);
            serializer.AddSerializedData("IK Mode", (int)IkMode);
            serializer.AddSerializedData("Duration", Duration);
            serializer.AddSerializedData("Animation Speed", AnimationSpeed);
            serializer.AddSerializedData("Max Weight", MaxWeight);
            serializer.AddSerializedData("Touch Human Bone", (int)TouchHumanBone);
            serializer.AddSerializedData("Touch Bone Name", TouchBoneName);
            serializer.AddSerializedData("Touch Bone", TouchBone != null ? TouchBone.gameObject : null);
            serializer.AddSerializedData("Target Object", TargetObject);
            serializer.AddSerializedData("Interactable Object", AssociatedInteractable != null ? AssociatedInteractable.gameObject : null);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            LayerName = serializer.GetSerializedString("Layer Name");
            ClipToOverride = serializer.GetSerializedString("Clip to Override");
            StateName = serializer.GetSerializedString("State Name");
            IkMode = (InteractionIKMode)serializer.GetSerializedInt("IK Mode");
            Duration = serializer.GetSerializedFloat("Duration");
            AnimationSpeed = serializer.GetSerializedFloat("Animation Speed");
            MaxWeight = serializer.GetSerializedFloat("Max Weight");
            if (MaxWeight <= 0f && serializer.GetSerializedString("Max Weight") == null)
            {
                MaxWeight = 1.0f;
            }
            TouchHumanBone = (HumanBodyBones)serializer.GetSerializedInt("Touch Human Bone");
            TouchBoneName = serializer.GetSerializedString("Touch Bone Name");
            GameObject touchBoneGo = serializer.GetSerializedGameobjects("Touch Bone");
            if (touchBoneGo != null)
            {
                TouchBone = touchBoneGo.transform;
            }
            TargetObject = serializer.GetSerializedGameobjects("Target Object");

            GameObject interactableGo = serializer.GetSerializedGameobjects("Interactable Object");
            if (interactableGo != null)
            {
                AssociatedInteractable = interactableGo.GetComponent<Interactable>();
            }
        }
    }
}
