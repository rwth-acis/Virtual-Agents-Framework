using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Component for synchronizing the agent's movement with the shown animation
    /// </summary>
    public class AgentAnimationUpdater : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;
        private float lastKnownRotation = 0;


        // animation Parameter Names
        /// <summary>
        /// Controls the forward speed.
        /// </summary>
        [Tooltip("Controls the forward speed.")]
        [SerializeField] private string forwardSpeed = "Speed";

        /// <summary>
        /// Controls the angular speed.
        /// </summary>
        [Tooltip("Controls the angular speed.")]
        [SerializeField] private string angularSpeed = "Turn";
        [SerializeField] private string rotationDirection = "RotationDirection";
        [SerializeField] private string isRotating = "IsRotating";

        // animation IDs
        private int _animIDSpeed;
        private int _animIDAngularSpeed;
        private int _animIDRotationDirection;
        private int _animIDIsRotating;

        private void Awake()
        {
            AssignAnimationIDs();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash(forwardSpeed);
            _animIDAngularSpeed = Animator.StringToHash(angularSpeed);
            _animIDRotationDirection = Animator.StringToHash(rotationDirection);
            _animIDIsRotating = Animator.StringToHash(isRotating);
        }



        // Updates the animation parameters for the blend trees
        private void UpdateAnimatorParameters()
        {
            animator.SetFloat(_animIDSpeed, agent.velocity.magnitude);
            float rotation = 0;

            if (agent.transform.rotation.eulerAngles.y > lastKnownRotation)
            {
                rotation = 1;
            }
            else if (agent.transform.rotation.eulerAngles.y < lastKnownRotation)
            {
                rotation = -1;
            }

            rotation = agent.velocity.magnitude > 0 ? 0 : rotation;

            animator.SetFloat(_animIDRotationDirection, rotation);
            animator.SetBool(_animIDIsRotating, rotation != 0);
            lastKnownRotation = agent.transform.rotation.eulerAngles.y;
        }

        private void Update()
        {
            UpdateAnimatorParameters();
        }
    }
}