using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace i5.VirtualAgents.AgentTasks
{
    public enum SittingDirection
    {
        SITDOWN,
        STANDUP,
        TOGGLE
    }

    public class AgentSittingTask: AgentBaseTask, ISerializable
    {
        private static readonly int Sitting = Animator.StringToHash("Sitting");

        /// <summary>
        /// The direction of the sitting task
        /// Can be sit down, stand up or toggle
        /// SITDOWN: the agent will sit down or stay sitting
        /// STANDUP: the agent will stand up or stay standing
        /// TOGGLE: if the agent is sitting, it will stand up; if the agent is standing, it will sit down
        /// </summary>
        public SittingDirection Direction { get; protected set; }
        /// <summary>
        /// The chair the agent should sit on
        /// </summary>
        public Chair Chair{ get; protected set; }

        private bool sitting = false;
        private float _animationDuration = 2.233f;
        /// <summary>
        /// How long the sitting down / standing up animation takes, to synchronize the IK fade and task length with the animation (in seconds).
        /// </summary>
        public float animationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Mathf.Max(0f, value);
        }
        private float _animationSitReached = 0.70f;
        /// <summary>
        /// When does the animation reach the sitting pose i.e. the hip is stable (in percent).
        /// </summary>
        public float animationSitReached
        {
            get => _animationSitReached;
            set => _animationSitReached = Mathf.Clamp01(value);
        }
        private bool finished = false;
        private bool failed = false;
        private TwoBoneIKConstraint leftLegIK;
        private Transform leftLegIKTarget;
        private TwoBoneIKConstraint rightLegIK;
        private Transform rightLegIKTarget;
        private MultiAimConstraint spineAim;
        private MultiParentConstraint hipConstraint;
        private Transform hipIKTarget;
        private Vector3 prevPosition;

        // For serialization purposes
        public AgentSittingTask()
        {
        }
        
        /// <summary>
        /// Enables the agent to sit on a prepared chair GameObject
        /// </summary>
        /// <param name="chair">The chair to be sat on. Needs to have at least "FeetPosition" and "SitPosition" child objects.</param>
        /// <param name="direction">Either SITDOWN, STANDUP or TOGGLE. TOGGLE is equivalent to SITDOWN while standing and STANDUP while sitting.</param>

        public AgentSittingTask(Chair chair, SittingDirection direction = SittingDirection.TOGGLE)
        {
            Direction = direction;
            Chair = chair;
        }

        public override void StartExecution(Agent agent)
        {
            finished = false;
            failed = false;
            if (Chair == null)
            {
                  Debug.LogWarning("No Chair assigned to AgentSittingTask. Aborting sitting task.");
                  failed = true;
                 return;
            }

            if(Chair.SeatedHipPosition == null || Chair.StandingFeetPosition == null)
            {
                Debug.LogWarning("The chair "+ Chair.name +" assigned to the AgentSittingTask does not have all necessary alignment points (SeatedHipPosition and StandingFeetPosition) assigned. Aborting sitting task.");
                failed = true;
                return;
            }
            
            Animator animator = agent.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"No Animator component found on agent {agent.name}. Aborting sitting task.");
                failed = true;
                return;
            }
            sitting = animator.GetBool(Sitting);
            bool oldState = sitting;

            if (Direction==SittingDirection.TOGGLE)
            {
                // toggle the sitting state
                sitting = !sitting;
            }
            else if(Direction==SittingDirection.SITDOWN)
            {
                // if no toggle set, sit down if not already sitting
                sitting = true;
            }
            else
            {
                // if no toggle set, stand up if not already standing
                sitting = false;
            }

            bool currentState = sitting;
            // check if animation is needed
            if (oldState != currentState)
            {
                MeshSockets agentSockets = agent.GetComponent<MeshSockets>();

                if (!agentSockets)
                {
                    Debug.LogWarning("No MeshSockets component found on agent. Aborting sitting task.");
                    failed = true;
                    return;
                }
                if(!agentSockets.VerifySetUpOfAllConstraints(false, true, true))
                {
                    Debug.LogWarning("The agent's MeshSockets component is not set up properly. Aborting sitting task.");
                    failed = true;
                    return;
                }
                
                // get all constraints
                leftLegIK = agentSockets.TwoBoneIKConstraintLeftLeg;
                leftLegIKTarget = leftLegIK.data.target;
                rightLegIK = agentSockets.TwoBoneIKConstraintRightLeg;
                rightLegIKTarget = rightLegIK.data.target;
                spineAim = agentSockets.MultiAimConstraintSpine;
                hipConstraint = agentSockets.MultiParentConstraintHip;
                hipIKTarget = hipConstraint.data.sourceObjects.GetTransform(0);
                
                

                // case: sitting down
                if (currentState)
                {
                    agent.StartCoroutine(RotateOverTime(agent, Chair.SeatedHipPosition.transform.rotation));
                    hipIKTarget.transform.position = Chair.SeatedHipPosition.position;

                    animator.SetBool(Sitting, sitting);

                    agent.StartCoroutine(FadeIK(agent, true));
                }
                // case: standing up
                else
                {
                    animator.SetBool(Sitting, false);
                    agent.StartCoroutine(FadeIK(agent, false));
                }

            }
            else
            {
                // Agent is already doing what it is instructed to do
                finished = true;
            }

        }

        /// <summary>
        /// This method fades IK in or out during the sit down / stand up animations. It also slowly moves the agent towards the chair or the feet position.
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="fadeIn">Whether to fade in or fade out the IK, etc. In other words: whether the agent sits down (true) or stands up (false)</param>
        /// <returns></returns>
        private IEnumerator FadeIK(Agent agent, bool fadeIn)
        {
            float fadeDuration = animationDuration * animationSitReached;
            float waitBeforeFade = fadeIn ? 0f : animationDuration * (1f - animationSitReached);
            float waitAfterFade = Mathf.Max(0f, animationDuration - waitBeforeFade - fadeDuration);
            float elapsed = 0f;
            float startWeight = fadeIn ? 0f : 1f;
            float endWeight = fadeIn ? 1f : 0f;
            Vector3 ikPosition = fadeIn ? Chair.SeatedFeetPosition.position : Chair.StandingFeetPosition.position;
            
            // When standing up, wait for the hip to leave the chair before starting to fade
            while (elapsed < waitBeforeFade)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float fadeProgress = fadeDuration > 0f ? elapsed / fadeDuration : 1f;
                leftLegIK.weight = Mathf.SmoothStep(startWeight, endWeight, fadeProgress);
                rightLegIK.weight = Mathf.SmoothStep(startWeight, endWeight, fadeProgress);

                // move ik target when standing up, to avoid, that the agent suddenly fully stretches their legs
                Vector3 curIkPosition =
                    fadeIn ? ikPosition : Vector3.Lerp(Chair.SeatedFeetPosition.position, Chair.StandingFeetPosition.position, fadeProgress);
                leftLegIKTarget.position = curIkPosition - Chair.SeatedFeetPosition.right * Chair.distanceBetweenFeet/2;
                rightLegIKTarget.position = curIkPosition + Chair.SeatedFeetPosition.right * Chair.distanceBetweenFeet/2;

                spineAim.weight = Mathf.SmoothStep(startWeight, endWeight, fadeProgress);
                hipConstraint.weight = Mathf.SmoothStep(startWeight, endWeight, fadeProgress);
                hipIKTarget.position = Chair.SeatedHipPosition.position;
                yield return null;
            }
            leftLegIK.weight = endWeight;
            rightLegIK.weight = endWeight;
            spineAim.weight = endWeight;
            hipConstraint.weight = endWeight;
            leftLegIKTarget.position = ikPosition - Chair.SeatedFeetPosition.right * Chair.distanceBetweenFeet/2;
            rightLegIKTarget.position = ikPosition + Chair.SeatedFeetPosition.right * Chair.distanceBetweenFeet/2;
            hipIKTarget.position = Chair.SeatedHipPosition.position;
            
            // When sitting down, wait for the animation to finish completely
            elapsed = 0f;
            while (fadeIn && elapsed < waitAfterFade)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            finished = true;

        }

        /// <summary>
        /// Smoothly rotate the agent to align its back to the chair
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="targetRotation">The target rotation to rotate towards</param>
        /// <param name="duration">The duration of the rotation</param>
        /// <returns></returns>
        private IEnumerator RotateOverTime(Agent agent, Quaternion targetRotation, float duration = 1f)
        {
            float time = 0;
            Quaternion startRotation = agent.transform.rotation;

            while (time < duration)
            {
                time += Time.deltaTime;
                agent.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
                yield return null;
            }

            agent.transform.rotation = targetRotation;
        }

        public override TaskState EvaluateTaskState()
        {
            if (finished)
            {
                FinishTask();
                return TaskState.Success;
            }

            if (failed)
            {
                FinishTaskAsFailed();
                return TaskState.Failure;
            }
            return TaskState.Running;
        }
        
        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Chair", Chair != null ? Chair.gameObject : null);
            serializer.AddSerializedData("Direction", (int)Direction);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            GameObject chairObject = serializer.GetSerializedGameobjects("Chair");
            Chair = chairObject != null ? chairObject.GetComponent<Chair>() : null;
            Direction = (SittingDirection)serializer.GetSerializedInt("Direction");
        }
    }
}