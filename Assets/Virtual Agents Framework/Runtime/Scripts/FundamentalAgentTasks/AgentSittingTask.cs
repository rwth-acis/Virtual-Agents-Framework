using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace i5.VirtualAgents.AgentTasks
{
    public class AgentSittingTask: AgentBaseTask, ISerializable
    {
        public bool Toggle { get; set; }
        private bool sitting = false;
        private float animationDuration = 2.233f;
        public GameObject Chair{ get; protected set; }
        private RigBuilder rigBuilder;
        private TwoBoneIKConstraint leftLegIK;
        private TwoBoneIKConstraint rightLegIK;
        private MultiAimConstraint spineAim;

        public AgentSittingTask(GameObject chair, bool sitDown, bool toggle = true)
        {
            // if sitDown is false the agent will stand up or keep standing
            Toggle = toggle;
            Chair = chair;
        }
        public override void StartExecution(Agent agent)
        {
            bool oldState = sitting;
            Animator animator = agent.GetComponent<Animator>();
            Vector3 sittingPosition = FindSittingSpace(Chair);
            agent.transform.position = sittingPosition;
            agent.transform.rotation = Chair.transform.rotation;
            if (Toggle)
            {
                // toggle the sitting state
                animator.SetBool("Sitting", !sitting);
            }
            else if(!sitting)
            {
                // if no toggle set, sit down if not already sitting
                animator.SetBool("Sitting", true);
            }

            bool currentState = animator.GetBool("Sitting");
            // check if animation is needed
            if (oldState != currentState)
            {
                // case: sitting down
                if (currentState == true)
                {
                    animator.SetFloat("SittingDirection", -1);
                    Debug.Log("Sitting down");
                }
                // case: standing up
                else
                {
                    animator.SetFloat("SittingDirection", 1);
                }
                // wait for animation to finish, then set state to sitting idle
                agent.StartCoroutine(WaitForAnimation(animator));
            }
        }

        private IEnumerator WaitForAnimation(Animator animator)
        {
            yield return new WaitForSeconds(animationDuration);
            animator.SetFloat("SittingDirection", 0);
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
            serializer.AddSerializedData("Toggle", Toggle);
            serializer.AddSerializedData("Sitting", sitting);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            animationDuration = serializer.GetSerializedFloat("Animation Duration");
            Toggle = serializer.GetSerializedBool("Toggle");
            sitting = serializer.GetSerializedBool("Sitting");
        }
    }
}