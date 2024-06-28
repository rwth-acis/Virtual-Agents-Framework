using UnityEngine;

namespace i5.VirtualAgents.Utilities
{
    public class DebugDrawTransformSphere : MonoBehaviour
    {
        /// <summary>
        /// The color of the sphere.
        /// </summary>
        [Tooltip("The color of the sphere.")]
        [SerializeField]
        public Color color = Color.green;

        /// <summary>
        /// The radius of the sphere
        /// </summary>
        [Tooltip("The radius of the sphere")]
        [SerializeField]
        public float radius = 0.05f;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}