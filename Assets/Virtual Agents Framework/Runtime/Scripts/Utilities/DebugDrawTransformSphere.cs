using UnityEngine;

namespace i5.VirtualAgents.Utilities
{
    public class DebugDrawTransformSphere : MonoBehaviour
    {
        [SerializeField]
        public Color color = Color.green;
        [SerializeField]
        public float radius = 0.05f;
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}