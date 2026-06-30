using System;
using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents.Examples
{
    /// <summary>
    /// Animates the tires of a vehicle or wheelchair based on the movement of a NavMeshAgent.
    /// Uses differential drive math to rotate the left and right rear wheels independently during turns,
    /// and animates the front caster wheels according to linear speed.
    /// 
    /// Note: The script is not completely accurate and only for demonstration purposes.
    /// </summary>
    public class TireAnimation : MonoBehaviour
    {
        [Header("Wheel Transforms")]
        [SerializeField] private Transform LeftTire;
        [SerializeField] private Transform RightTire;
        [SerializeField] private Transform LeftFrontTire;
        [SerializeField] private Transform RightFrontTire;
        
        [NonSerialized] public NavMeshAgent agent;

        [Header("Wheel Settings")]
        [Tooltip("Check this if the wheels are spinning backwards")]
        [SerializeField] private bool invertRotation = true;

        // Radius of the large rear wheels
        private float rearWheelRadius;
        // Radius of the small front caster wheels
        private float frontWheelRadius;
        // Distance between the left and right rear wheels
        private float trackWidth;

        // Variable to track rotation delta
        private float lastRotationY;

        void Start()
        {
            // Calculate Track Width and Rear Radius
            if (LeftTire != null && RightTire != null)
            {
                trackWidth = Vector3.Distance(LeftTire.position, RightTire.position);
                rearWheelRadius = GetWheelRadius(LeftTire);
            }
            else
            {
                Debug.LogWarning("Left or Right rear tire is unassigned. Cannot calculate Track Width.");
            }

            // Calculate Front Caster Radius
            if (LeftFrontTire != null)
            {
                frontWheelRadius = GetWheelRadius(LeftFrontTire);
            }
            else if (RightFrontTire != null)
            {
                frontWheelRadius = GetWheelRadius(RightFrontTire);
            }
            
            // Initialize tracking variable
            lastRotationY = transform.eulerAngles.y;
        }
        
        private float GetWheelRadius(Transform wheel)
        {
            Renderer renderer = wheel.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Vector3 extents = renderer.bounds.extents;
                return Mathf.Max(extents.x, extents.y, extents.z);
            }
            
            Debug.LogWarning($"No Renderer found on {wheel.name}. Defaulting radius to 0.3f");
            return 0.3f;
        }

        void LateUpdate()
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                // Determine direction multiplier based on the inspector toggle
                float dir = invertRotation ? -1f : 1f;

                // 1. Calculate linear speed directly from the agent's velocity
                float forwardDistance = agent.velocity.magnitude * Time.deltaTime;

                // 2. Calculate angular movement (turning)
                float deltaAngle = Mathf.DeltaAngle(lastRotationY, transform.eulerAngles.y);
                float deltaAngleRad = deltaAngle * Mathf.Deg2Rad; 

                // 3. Differential drive math for rear wheels
                float turnDistanceOffset = (deltaAngleRad * trackWidth) / 2f;

                float leftDistance = forwardDistance + turnDistanceOffset;
                float rightDistance = forwardDistance - turnDistanceOffset;

                float leftRotationDegrees = (leftDistance / rearWheelRadius) * Mathf.Rad2Deg;
                float rightRotationDegrees = (rightDistance / rearWheelRadius) * Mathf.Rad2Deg;

                // 4. Apply rotation to rear wheels (multiplied by our direction toggle)
                if (LeftTire) LeftTire.Rotate(Vector3.up, leftRotationDegrees * dir, Space.Self);
                if (RightTire) RightTire.Rotate(Vector3.up, rightRotationDegrees * dir, Space.Self);

                // 5. Apply rotation to front casters
                if (forwardDistance > 0.0001f)
                {
                    float frontRotationDegrees = (forwardDistance / frontWheelRadius) * Mathf.Rad2Deg;

                    if (LeftFrontTire) LeftFrontTire.Rotate(Vector3.up, frontRotationDegrees * dir, Space.Self);
                    if (RightFrontTire) RightFrontTire.Rotate(Vector3.up, frontRotationDegrees * dir, Space.Self);
                }
            }

            // Update rotation tracker for the next frame
            lastRotationY = transform.eulerAngles.y;
        }
    }
}