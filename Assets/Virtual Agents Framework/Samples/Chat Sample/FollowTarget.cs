using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target; // Reference to the target GameObject to follow
        public Vector3 offset;  // The offset in each axis (X, Y, Z)

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                // Calculate the new position by adding the offset to the target's position
                Vector3 newPosition = target.position + offset;

                // Set the position of this GameObject to the new position
                transform.position = newPosition;
            }
            else
            {
                Debug.LogWarning("Target GameObject is not set in FollowTargetWithOffsets script on " + name + "!");
            }
        }
    }
}
