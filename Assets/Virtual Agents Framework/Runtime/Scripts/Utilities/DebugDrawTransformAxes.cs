using UnityEngine;

namespace i5.VirtualAgents.Utilities

{
    public class DebugDrawTransformAxes : MonoBehaviour
    {
        [SerializeField] private bool drawGreenY = false;
        [SerializeField] private bool drawRedX = false;
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