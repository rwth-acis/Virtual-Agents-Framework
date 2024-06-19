using UnityEngine;

namespace i5.VirtualAgents.Utilities
{
    public class DebugDrawTransformSphere : MonoBehaviour
    {
        [Tooltip("The color of the sphere")]
        [SerializeField]
        public Color color = Color.green;
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