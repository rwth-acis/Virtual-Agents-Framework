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
        public SittingDirection Direction { get; protected set; }
        private bool sitting = false;
        private float animationDuration = 2.233f;
        private bool finished = false;
        public GameObject Chair{ get; protected set; }
        private RigBuilder rigBuilder;
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
                rigBuilder = agent.transform.Find("AnimationRigging/CharacterRig").GetComponent<RigBuilder>();
                leftLegIK = agent.transform.Find("AnimationRigging/CharacterRig/Left Leg IK").GetComponent<TwoBoneIKConstraint>();
                leftLegIKTarget = leftLegIK.transform.Find("Left Leg IK_target").gameObject;
                rightLegIK = agent.transform.Find("AnimationRigging/CharacterRig/Right Leg IK").GetComponent<TwoBoneIKConstraint>();
                rightLegIKTarget = rightLegIK.transform.Find("Right Leg IK_target").gameObject;
                spineAim = agent.transform.Find("AnimationRigging/CharacterRig/Spine Aim").GetComponent<MultiAimConstraint>();
                hipConstraint = agent.transform.Find("AnimationRigging/CharacterRig/Hip Constraint").GetComponent<MultiParentConstraint>();
                hipIKTarget = hipConstraint.transform.Find("Hip IK_target").gameObject;

                // set the target positions to the agents current position
                leftLegIK.transform.position = rightLegIK.transform.position = agent.transform.position;
                hipIKTarget.transform.position = agent.transform.position;

                // case: sitting down
                if (currentState)
                {
                    agent.transform.rotation = Chair.transform.rotation;
                    Debug.Log("Setting footrest and sitposition");
                    leftLegIKTarget.transform.position = rightLegIKTarget.transform.position = footrest;
                    hipIKTarget.transform.position = sitPosition;

                    // enable constraints
                    animator.SetBool("Sitting", sitting);

                    // wait for animation to finish, then set state to sitting idle
                    agent.StartCoroutine(FadeIKAndPosition(agent, true));

                    Debug.Log("Sitting down");
                }
                // case: standing up
                else
                {
                    Debug.Log(animator.GetBool("Sitting"));
                    leftLegIKTarget.transform.position = rightLegIKTarget.transform.position = feetPosition;
                    animator.SetBool("Sitting", false);
                    agent.StartCoroutine(FadeIKAndPosition(agent, false));

                    Debug.Log("Standing up");
                }

            }

        }

        private IEnumerator FadeIKAndPosition(Agent agent, bool fadeIn)
        {
            float duration = animationDuration;
            float time = 0;
            float startWeight = fadeIn ? 0 : 1;
            float endWeight = fadeIn ? 1 : 0;
            Vector3 ikPosition = fadeIn ? footrest : feetPosition;
            Vector3 startPosition = agent.transform.position;
            Vector3 endPosition = fadeIn ? sitPosition : feetPosition;
            while (time < duration)
            {
                rightLegIK.gameObject.transform.position = leftLegIK.gameObject.transform.position = footrest;
                time += Time.deltaTime;
                leftLegIK.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                rightLegIK.weight = Mathf.Lerp(startWeight, endWeight, time / duration);

                // move ik target when standing up, to avoid, that the agent suddenly completely stretches their feet
                leftLegIKTarget.transform.position = fadeIn
                    ? rightLegIKTarget.transform.position = ikPosition
                    : rightLegIKTarget.transform.position = Vector3.Lerp(footrest, feetPosition, time / duration);

                spineAim.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                hipConstraint.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                agent.transform.position = Vector3.Lerp(startPosition, endPosition, time / duration);
                hipIKTarget.transform.position = sitPosition;
                yield return null;
            }

            rightLegIK.gameObject.transform.position = leftLegIK.gameObject.transform.position = footrest;
            leftLegIKTarget.transform.position = rightLegIKTarget.transform.position = ikPosition;
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