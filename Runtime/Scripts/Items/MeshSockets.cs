using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSockets : MonoBehaviour
{
    public enum SocketId
    {
        Spine,
        RightHand,
        LeftHand
    }

    Dictionary<SocketId, MeshSocket> socketMap = new Dictionary<SocketId, MeshSocket>();
    // Start is called before the first frame update
    void Start()
    {
        //Collect all the MeshSockets in the agent
        MeshSocket[] sockets = GetComponentsInChildren<MeshSocket>();
        foreach (MeshSocket socket in sockets)
        {
            socketMap[socket.socketId] = socket;
        }
    }

    public void Attach(Transform objectTransform, SocketId socketId)
    {
        socketMap[socketId].Attach(objectTransform);
    }
}
