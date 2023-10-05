using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugDrawLine : MonoBehaviour
{
    public string arrow = "Green";
    private void OnDrawGizmos()
    {
        if (this.arrow == "Green")
            Debug.DrawLine(transform.position, transform.position + transform.up * 50, Color.green);
        if (this.arrow == "Red")
            Debug.DrawLine(transform.position, transform.position + transform.right * 50, Color.red);
        if (this.arrow == "Blue")
            Debug.DrawLine(transform.position, transform.position + transform.forward * 50, Color.blue);
    }
}
