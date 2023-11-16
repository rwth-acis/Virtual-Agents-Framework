using i5.VirtualAgents;
using System.Collections;
using UnityEngine;

public class MeshSocket : MonoBehaviour
{
    Transform attachPointOffset;
    public MeshSockets.SocketId socketId;
    // Start is called before the first frame update
    void Start()
    {
        // Get offset as attachPoint
        attachPointOffset = transform.GetChild(0);
    }

    public void Attach(Item item)
    {
        // item.grapTarget should be at the same position as the attachPoint
        item.transform.SetParent(attachPointOffset, false);
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Calculate the diffrerence between the attachPoint and the item's grapTarget
        Vector3 offsetPosition = attachPointOffset.position - item.grapTarget.transform.position;
        Quaternion offsetRotation = Quaternion.Inverse(attachPointOffset.transform.rotation) * item.grapTarget.transform.rotation;

        //Move the attachPointOffset so that the attachPoint allignes with the attachPoint
        attachPointOffset.transform.SetLocalPositionAndRotation(offsetPosition, offsetRotation);

        StartCoroutine(StopFurtherMovement(item.transform));
    }

    // In case there is any movement that hasn't stopped yet, local position and rotation will be set to zero as long as the object is moving
    public IEnumerator StopFurtherMovement(Transform objectTransform)
    {
        yield return new WaitForSeconds(0.025f);
        while (objectTransform.localPosition != Vector3.zero)
        {
            objectTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            yield return new WaitForSeconds(0.025f);
        }
    }
}
