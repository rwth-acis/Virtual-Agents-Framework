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
        StartCoroutine(stopFurtherMovement(objectTransform));
    }

    //Incase there is any movement that hasn't stopped yet, local position and rotation will be set to zero as long as the object is moving
    public IEnumerator stopFurtherMovement(Transform objectTransform)
    {
        yield return new WaitForSeconds(0.025f);
        while (objectTransform.localPosition != Vector3.zero)
        {
            objectTransform.localPosition = Vector3.zero;
            objectTransform.localRotation = Quaternion.identity;
        }
    }

    public void detach(Transform objectTransform)
    {
        objectTransform.SetParent(null, true);
    }
}
