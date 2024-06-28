using UnityEngine;

namespace i5.VirtualAgents.Utilities

{
    public class DebugDrawTransformAxes : MonoBehaviour
    {
        /// <summary>
        /// Draw the green Y axis of the transform.
        /// </summary>
        [Tooltip("Draw the green Y axis of the transform.")]
        [SerializeField] private bool drawGreenY = false;

        /// <summary>
        /// Draw the red X axis of the transform.
        /// </summary>
        [Tooltip("Draw the red X axis of the transform.")]
        [SerializeField] private bool drawRedX = false;

        /// <summary>
        /// Draw the blue Z axis of the transform.
        /// </summary>
        [Tooltip("Draw the blue Z axis of the transform.")]
        [SerializeField] private bool drawBlueZ = true;

        private void OnDrawGizmos()
        {
            if (drawGreenY)
                Debug.DrawLine(transform.position, transform.position + transform.up * 50, Color.green);
            if (drawRedX)
                Debug.DrawLine(transform.position, transform.position + transform.right * 50, Color.red);
            if (drawBlueZ)
                Debug.DrawLine(transform.position, transform.position + transform.forward * 50, Color.blue);
        }
    }
}