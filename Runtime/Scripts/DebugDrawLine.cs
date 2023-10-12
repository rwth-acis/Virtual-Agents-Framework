using UnityEngine;

public class DebugDrawLine : MonoBehaviour
{
    [SerializeField] private bool drawGreenY;
    [SerializeField] private bool drawRedX;
    [SerializeField] private bool drawBlueZ;

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
