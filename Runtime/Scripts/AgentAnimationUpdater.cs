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
        [SerializeField] private string sittingDirection = "SittingDirection";
        [SerializeField] private string sitting = "Sitting";

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
        
        // expected maximum turning speed in degrees per second, used for normalizing the rotation parameter
        private float maxAngularTurnSpeed = 360f;
        public float MaxAngularTurnSpeed { set => maxAngularTurnSpeed = value; }

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

            float currentY = agent.transform.rotation.eulerAngles.y;
            float delta = Mathf.DeltaAngle(lastKnownRotation, currentY); // using DeltaAngle to get the shortest angle difference, handling wrap-around between 0 -> 360 degrees

            // convert delta (degrees per frame) to degrees per second
            float degPerSec = delta / Mathf.Max(Time.deltaTime, 1e-6f);

            // normalize to -1..1 using an expected max turning speed
            float targetRotation = Mathf.Clamp(degPerSec / maxAngularTurnSpeed, -1f, 1f);

            // suppress rotation while moving
            if (agent.velocity.magnitude > 0.01f)
                targetRotation = 0f;

            // smooth the rotation value for continuous transitions
            float prevRotation = animator.GetFloat(_animIDRotationDirection);
            const float smoothSpeed = 15f;
            float rotation = Mathf.Lerp(prevRotation, targetRotation, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));

            // snap to -1, 0, or 1 when close enough to avoid blending around those values
            rotation = Mathf.Abs(rotation) > 0.98f ? Mathf.Sign(rotation) : (Mathf.Abs(rotation) < 0.02f ? 0f : rotation);

            animator.SetFloat(_animIDRotationDirection, rotation);
            animator.SetBool(_animIDIsRotating, Mathf.Abs(rotation) > 0.01f);
            lastKnownRotation = currentY;
        }

        private void Update()
        {
            UpdateAnimatorParameters();
        }
    }
}