using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

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
        public GameObject Chair{ get; protected set; }

        private bool sitting = false;
        private float animationDuration = 2.233f;
        private bool finished = false;
        private TwoBoneIKConstraint leftLegIK;
        private GameObject leftLegIKTarget;
        private TwoBoneIKConstraint rightLegIK;
        private GameObject rightLegIKTarget;
        private MultiAimConstraint spineAim;
        private MultiParentConstraint hipConstraint;
        private GameObject hipIKTarget;
        private Vector3 prevPosition;
        private readonly Vector3 feetPosition;
        private readonly Vector3 footrest;
        private readonly Vector3 sitPosition;

        public AgentSittingTask(GameObject chair, SittingDirection direction = SittingDirection.TOGGLE)
        {
            Direction = direction;
            Chair = chair;
            // TODO: give as parameters
            feetPosition = chair.transform.Find("FeetPosition").position;
            footrest = chair.transform.Find("Footrest") != null ? chair.transform.Find("Footrest").position : feetPosition;
            sitPosition = chair.transform.Find("SitPosition").position;
        }

        public override void StartExecution(Agent agent)
        {
            Animator animator = agent.GetComponent<Animator>();
            sitting = animator.GetBool("Sitting");
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
                // get all constraints
                leftLegIK = agent.transform.Find("AnimationRigging/CharacterRig/Left Leg IK").GetComponent<TwoBoneIKConstraint>();
                leftLegIKTarget = leftLegIK.transform.Find("Left Leg IK_target").gameObject;
                rightLegIK = agent.transform.Find("AnimationRigging/CharacterRig/Right Leg IK").GetComponent<TwoBoneIKConstraint>();
                rightLegIKTarget = rightLegIK.transform.Find("Right Leg IK_target").gameObject;
                spineAim = agent.transform.Find("AnimationRigging/CharacterRig/Spine Aim").GetComponent<MultiAimConstraint>();
                hipConstraint = agent.transform.Find("AnimationRigging/CharacterRig/Hip Constraint").GetComponent<MultiParentConstraint>();
                hipIKTarget = hipConstraint.transform.Find("Hip IK_target").gameObject;

                leftLegIK.transform.position = rightLegIK.transform.position = Vector3.zero;
                hipIKTarget.transform.position = Vector3.zero;

                // case: sitting down
                if (currentState)
                {
                    agent.transform.rotation = Chair.transform.rotation;
                    hipIKTarget.transform.position = sitPosition;

                    animator.SetBool("Sitting", sitting);

                    agent.StartCoroutine(FadeIK(agent, true));
                }
                // case: standing up
                else
                {
                    animator.SetBool("Sitting", false);
                    agent.StartCoroutine(FadeIK(agent, false));

                    Debug.Log("Standing up");
                }

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
            float duration = animationDuration;
            float time = 0;
            float startWeight = fadeIn ? 0 : 1;
            float endWeight = fadeIn ? 1 : 0;
            Vector3 ikPosition = fadeIn ? footrest : feetPosition;
            Vector3 curIkPosition = ikPosition;

            while (time < duration)
            {
                rightLegIK.gameObject.transform.position = leftLegIK.gameObject.transform.position = feetPosition;
                time += Time.deltaTime;
                leftLegIK.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                rightLegIK.weight = Mathf.Lerp(startWeight, endWeight, time / duration);

                // move ik target when standing up, to avoid, that the agent suddenly fully stretches their legs
                curIkPosition =
                    fadeIn ? ikPosition : Vector3.Lerp(footrest, feetPosition, time / duration);
                leftLegIKTarget.transform.position = curIkPosition - agent.transform.right * 0.08f;
                rightLegIKTarget.transform.position = curIkPosition + agent.transform.right * 0.08f;

                spineAim.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                hipConstraint.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                hipIKTarget.transform.position = sitPosition;
                yield return null;
            }
            rightLegIK.gameObject.transform.position = leftLegIK.gameObject.transform.position = feetPosition;
            leftLegIKTarget.transform.position = ikPosition - agent.transform.right * 0.08f;
            rightLegIKTarget.transform.position = ikPosition + agent.transform.right * 0.08f;
            hipIKTarget.transform.position = sitPosition;

            finished = true;

        }

        public override TaskState EvaluateTaskState()
        {
            if (finished)
            {
                return TaskState.Success;
            }
            return TaskState.Running;
        }


        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Animation Duration", animationDuration);
            //serializer.AddSerializedData("Toggle", Toggle);
            serializer.AddSerializedData("Sitting", sitting);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            animationDuration = serializer.GetSerializedFloat("Animation Duration");
            //Toggle = serializer.GetSerializedBool("Toggle");
            sitting = serializer.GetSerializedBool("Sitting");
        }
    }
}