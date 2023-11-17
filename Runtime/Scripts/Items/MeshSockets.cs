using i5.VirtualAgents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static MeshSockets;

public class MeshSockets : MonoBehaviour
{
    public enum SocketId
    {
        Spine,
        RightHand,
        LeftHand
    }

    /// <summary>
    /// Define the two bone IK constraints for the arms that is uses in the AgetPickUpTask
    /// </summary>
    public TwoBoneIKConstraint twoBoneIKConstraintRightArm;
    public TwoBoneIKConstraint twoBoneIKConstraintLeftArm;

    Dictionary<SocketId, MeshSocket> socketMap = new();
    // Start is called before the first frame update
    void Start()
    {
        // Collect all the MeshSockets in the agent
        MeshSocket[] sockets = GetComponentsInChildren<MeshSocket>();
        foreach (MeshSocket socket in sockets)
        {
            socketMap[socket.socketId] = socket;
        }
    }

    public void Attach(Item item, SocketId socketId)
    {
        socketMap[socketId].Attach(item);
    }
    public void Detach(Item item)
    {
        //Get the socketId of the socket that the item is attached to
        SocketId socketId = item.transform.parent.parent.GetComponent<MeshSocket>().socketId;

        socketMap[socketId].Detach(item);
    }
}
