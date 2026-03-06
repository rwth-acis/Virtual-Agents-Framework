using System;
using System.Collections;
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
        
        private const float smoothSpeedUp = 20f;
        private const float smoothSpeedDown = 5f;
        private const float smoothSpeedWalking = 25f;
        
        // expected maximum turning speed in degrees per second, used for normalizing the rotation parameter
        private float maxAngularTurnSpeed = 360f;
        public float MaxAngularTurnSpeed { set => maxAngularTurnSpeed = value; }
        
        // target direction set by the active rotation task
        private float rotationAnimationDirection = 0f;
        private float prevRotationBlending = 0;

		private void Awake()
        {
            AssignAnimationIDs();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;
            animator.SetFloat(_animIDRotationDirection, 0);
            animator.SetBool(_animIDIsRotating, false);
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
            float agentVelocityMag =  agent.velocity.magnitude;
            animator.SetFloat(_animIDSpeed, agentVelocityMag);
            
            // Rotation blending
            
            float rotAniDir = rotationAnimationDirection;
            
            // Blending the rotation animation differently for starting and stopping rotation
            float smoothSpeed = rotAniDir != 0 ? smoothSpeedUp : smoothSpeedDown;
            
            // Suppress rotation animation while moving
            if (agentVelocityMag > 0.01f)
            {
                rotAniDir = 0f;
                smoothSpeed = smoothSpeedWalking;
            }
            float rotationAnimationBlending = Mathf.Lerp(prevRotationBlending, rotAniDir, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));
            
            prevRotationBlending = rotationAnimationBlending; // Store rotation before snapping
            
            // Snap to -1, 0, or 1 when close enough to avoid blending around those values
            rotationAnimationBlending = Mathf.Abs(rotationAnimationBlending) > 0.95f ? Mathf.Sign(rotationAnimationBlending) : (Mathf.Abs(rotationAnimationBlending) < 0.05f ? 0f : rotationAnimationBlending);

            animator.SetFloat(_animIDRotationDirection, rotationAnimationBlending);
            animator.SetBool(_animIDIsRotating, Mathf.Abs(rotationAnimationBlending) > 0.01f);
        }
        
        /// <summary>
        /// Physically rotates the agent towards the target rotation
        /// </summary>
        public IEnumerator RotateTowardsTarget(Quaternion targetRotation, float speed, float threshold, Action onComplete)
        {
            // safe-guard against invalid speed values; using them would otherwise cause an infinite loop
            if (speed <= 0f)
            {
                transform.rotation = targetRotation;
                rotationAnimationDirection = 0f;
                onComplete?.Invoke();
                yield break;
            }

            // Determine target rotation direction for blending to the rotation animation in UpdateAnimatorParameters
            Vector3 cross = Vector3.Cross(transform.forward, targetRotation * Vector3.forward);
            rotationAnimationDirection = cross.y < 0 ? -1f : 1f;
            
            // Do the physical rotation according to speed
            while (Quaternion.Angle(transform.rotation, targetRotation) > threshold)
            {
                float step = speed * Time.deltaTime; // degrees to rotate this frame
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
                yield return null;
            }

            // Snap to target rotation to avoid tiny remaining differences
            transform.rotation = targetRotation;
            
            // Reset the target so the Update method can smoothly blend it back to 0
            rotationAnimationDirection = 0f;

            // Signal the task that the physical rotation is finished
            onComplete?.Invoke();
        }

        private void Update()
        {
            UpdateAnimatorParameters();
        }
    }
}