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
        private TwoBoneIKConstraint rightLegIK;
        private MultiAimConstraint spineAim;
        private MultiParentConstraint hipConstraint;
        private Vector3 prevPosition;
        private Vector3 feetPosition;
        private Vector3 sitPosition;

        public AgentSittingTask(GameObject chair, SittingDirection direction = SittingDirection.TOGGLE)
        {
            //TODO: replace with toggle
            Direction = direction;
            Chair = chair;
            feetPosition = chair.transform.Find("FeetPosition").position;
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
                rightLegIK = agent.transform.Find("AnimationRigging/CharacterRig/Right Leg IK").GetComponent<TwoBoneIKConstraint>();
                spineAim = agent.transform.Find("AnimationRigging/CharacterRig/Spine Aim").GetComponent<MultiAimConstraint>();
                hipConstraint = agent.transform.Find("AnimationRigging/CharacterRig/Hip Constraint").GetComponent<MultiParentConstraint>();

                // case: sitting down
                if (currentState)
                {
                    // move to chair position
                    Vector3 sittingPosition = FindSittingSpace(Chair);
                    //agent.transform.position = sittingPosition;
                    agent.transform.rotation = Chair.transform.rotation;

                    // enable constraints
                    animator.SetBool("Sitting", sitting);
                    animator.SetFloat("SittingDirection", -1);

                    leftLegIK.weight = 1;
                    rightLegIK.weight = 1;
                    spineAim.weight = 1;
                    hipConstraint.weight = 1;

                    // wait for animation to finish, then set state to sitting idle
                    agent.StartCoroutine(FadeIkAndPosition(agent, true));

                    Debug.Log("Sitting down");
                }
                // case: standing up
                else
                {
                    Debug.Log(animator.GetBool("Sitting"));
                    animator.SetFloat("SittingDirection", 1);
                    animator.SetBool("Sitting", false);
                    agent.StartCoroutine(FadeIkAndPosition(agent, false));

                    Debug.Log("Standing up");
                }

            }

        }


        private IEnumerator FadeIkAndPosition(Agent agent, bool fadeIn)
        {
            float duration = animationDuration;
            float time = 0;
            float startWeight = fadeIn ? 0 : 1;
            float endWeight = fadeIn ? 1 : 0;
            Vector3 startPosition = agent.transform.position;
            Vector3 endPosition = fadeIn ? sitPosition : feetPosition;
            while (time < duration)
            {
                time += Time.deltaTime;
                leftLegIK.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                rightLegIK.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                spineAim.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                hipConstraint.weight = Mathf.Lerp(startWeight, endWeight, time / duration);
                agent.transform.position = Vector3.Lerp(startPosition, endPosition, time / duration);
                yield return null;
            }
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
        private Vector3 FindSittingSpace(GameObject chair)
        {
            // Find the sitting space dynamically
            Collider chairCollider = chair.GetComponent<Collider>();
            if (chairCollider != null)
            {
                Vector3 sittingSpace = chairCollider.bounds.center;
                sittingSpace.y = chairCollider.bounds.min.y; // Adjust to the bottom of the chair
                return sittingSpace;
            }
            else
            {
                Debug.LogWarning("Chair collider not found, using chair position.");
                return chair.transform.position;
            }
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