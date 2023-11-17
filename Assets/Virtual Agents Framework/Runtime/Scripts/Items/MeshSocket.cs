using i5.VirtualAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static MeshSockets;

public class MeshSocket : MonoBehaviour
{
    public MeshSockets.SocketId socketId;

    private Dictionary<Item, Transform> offsetMap = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Attach(Item item)
    {
        //Create new offset for the new item
        Transform attachPointOffset = new GameObject().transform;
        attachPointOffset.name = "AttachPointOffsetBasedOnGrapTargetOf" + item.name;
        //Add rigTransform component to the offset object
        RigTransform rigTransform = attachPointOffset.gameObject.AddComponent<RigTransform>();

        offsetMap[item] = attachPointOffset;
        attachPointOffset.SetParent(this.transform);
        //item.grapTarget should be at the same position as the attachPoint
        item.transform.SetParent(attachPointOffset, false);
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        //Calculate the diffrerence between the attachPoint and the item's grapTarget
        Vector3 offsetPosition =  item.grapTarget.transform.position - attachPointOffset.position;
        Quaternion offsetRotation = Quaternion.Inverse(attachPointOffset.transform.rotation) * item.grapTarget.transform.rotation;
        //Move the attachPointOffset so that the attachPoint allignes with the attachPoint
        attachPointOffset.transform.SetLocalPositionAndRotation(offsetPosition, offsetRotation);



        StartCoroutine(StopFurtherMovement(item.transform));
    }

    public void Detach(Item item)
    {
        //Destroy the offset
        Destroy(offsetMap[item].gameObject);
        offsetMap.Remove(item);
        item.transform.SetParent(null, true);
        item.IsDropped();
    }

    //Incase there is any movement that hasn't stopped yet, local position and rotation will be set to zero as long as the object is moving
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
