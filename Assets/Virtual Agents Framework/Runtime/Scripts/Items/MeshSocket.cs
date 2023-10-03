using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSocket : MonoBehaviour
{
    Transform attachPoint;
    public MeshSockets.SocketId socketId;
    // Start is called before the first frame update
    void Start()
    {
        //Get offset as attachPoint
        attachPoint = transform.GetChild(0);
    }

    public void Attach(Transform objectTransform)
    {
        objectTransform.SetParent(attachPoint, false);
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;
    }
}
