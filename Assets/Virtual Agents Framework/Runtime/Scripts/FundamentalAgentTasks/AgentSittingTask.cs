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
        private float animationDuration = 2.233f; // How long the sitting down / standing up animation takes, to synchronize the IK fade with the animation, in seconds 
        private float animationSitReached = 0.70f; // When does the animation reach the sitting pose i.e. the hip is stable, in percent
        private bool finished = false;
        private bool failed =  false;
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
            if (Chair == null)
            {
                  Debug.LogWarning("No Chair assigned to AgentSittingTask. Aborting task.");
                  failed = true;
                 return;
            }

            if(Chair.SeatedHipPosition == null || Chair.StandingFeetPosition == null)
            {
                Debug.LogWarning("The chair "+ Chair.name +" assigned to the AgentSittingTask does not have all necessary alignment points (SeatedHipPosition and StandingFeetPosition) assigned. Aborting task.");
                failed = true;
                return;
            }
            
            Animator animator = agent.GetComponent<Animator>();
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
            // When sitting down up, completely fade before the hip reaches the chair
            float duration = animationDuration * animationSitReached;
            
            float time = 0;
            float startWeight = fadeIn ? 0 : 1;
            float endWeight = fadeIn ? 1 : 0;
            Vector3 ikPosition = fadeIn ? Chair.SeatedFeetPosition.position : Chair.StandingFeetPosition.position;
            
            // When standing up, wait for the hip to leave the chair before starting to fade
            while (!fadeIn && time < (duration * (1- animationSitReached)))
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                leftLegIK.weight = Mathf.SmoothStep(startWeight, endWeight, time / duration);
                rightLegIK.weight = Mathf.SmoothStep(startWeight, endWeight, time / duration);

                // move ik target when standing up, to avoid, that the agent suddenly fully stretches their legs
                Vector3 curIkPosition =
                    fadeIn ? ikPosition : Vector3.Lerp(Chair.SeatedFeetPosition.position, Chair.StandingFeetPosition.position, time / duration);
                leftLegIKTarget.position = curIkPosition - agent.transform.right * Chair.distanceBetweenFeet/2;
                rightLegIKTarget.position = curIkPosition + agent.transform.right * Chair.distanceBetweenFeet/2;

                spineAim.weight = Mathf.SmoothStep(startWeight, endWeight, time / duration);
                hipConstraint.weight = Mathf.SmoothStep(startWeight, endWeight, time / duration);
                hipIKTarget.position = Chair.SeatedHipPosition.position;
                yield return null;
            }
            leftLegIKTarget.position = ikPosition - agent.transform.right * Chair.distanceBetweenFeet/2;
            rightLegIKTarget.position = ikPosition + agent.transform.right * Chair.distanceBetweenFeet/2;
            hipIKTarget.position = Chair.SeatedHipPosition.position;

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
                return TaskState.Success;
            if(failed)
                return TaskState.Failure;
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