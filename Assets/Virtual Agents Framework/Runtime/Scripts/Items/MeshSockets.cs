using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace i5.VirtualAgents
{
	/// <summary>
	/// Collects and advertises the available mesh sockets to other scripts
	/// </summary>
	public class MeshSockets : MonoBehaviour
    {
        public enum SocketId
        {
            RightHand,
            LeftHand,
            RightLowerArm,
            LeftLowerArm,
            LeftUpperArm,
            RightUpperArm,
            RightBack,
            LeftBack,
            HipsFrontLeft,
            HipsFrontRight,
            HipsBackLeft,
            HipsBackRight,    
            AdditionalSocket1,
            AdditionalSocket2,
            AdditionalSocket3,
            AdditionalSocket4,
            AdditionalSocket5,
            AdditionalSocket6,
            AdditionalSocket7,
            AdditionalSocket8,
            AdditionalSocket9,
            AdditionalSocket10
        }

        /// <summary>
        /// Define the two bone IK constraints for the arms that is uses in the AgetPickUpTask
        /// </summary>
        [field: SerializeField]
        public TwoBoneIKConstraint TwoBoneIKConstraintRightArm { get; private set; }
        [field: SerializeField]
        public TwoBoneIKConstraint TwoBoneIKConstraintLeftArm { get; private set; }

        Dictionary<SocketId, MeshSocket> socketMap = new();
        Dictionary<Item, SocketId> itemSocketMap = new();
        // Initializes the component and collects the MeshSockets
        private void Start()
        {
            // Collect all the MeshSockets in the agent
            MeshSocket[] sockets = GetComponentsInChildren<MeshSocket>();
            foreach (MeshSocket socket in sockets)
            {
                //Check if the socket is already in the map
                if (socketMap.ContainsKey(socket.SocketId))
                {
                    Debug.LogError("Socket with ID " + socket.SocketId + " is defined multiple times in the agent.");
                }

                socketMap[socket.SocketId] = socket;
            }
        }

        /// <summary>
        /// Attaches an item to the socket with the given ID
        /// </summary>
        /// <param name="item">The item to attach</param>
        /// <param name="socketId">The ID by which the socket can be found</param>
        public void Attach(Item item, SocketId socketId)
        {
            if (socketMap.ContainsKey(socketId) && socketMap[socketId] != null)
            {
                // Attach the item to the socket
                socketMap[socketId].Attach(item);
                // Add the item to the itemSocketMap, this is needed because the socketID can have multiple Items attached to it
                itemSocketMap[item] = socketId;
            }
            else
            {
                Debug.LogError("Socket with ID " + socketId + " not found or is null.");
            }
        }

        /// <summary>
        /// Detaches an item from its socket
        /// </summary>
        /// <param name="item">The item to detach</param>
        public void Detach(Item item)
        {
            // Get the socketId of the socket that the item is attached to
            SocketId socketId = itemSocketMap[item];
            // Null check
            if (socketMap.ContainsKey(socketId) && socketMap[socketId] != null)
            {
                // Detach the item from the socket
                socketMap[socketId].Detach(item);
                // Remove the item from the itemSocketMap
                itemSocketMap.Remove(item);
            }
            else
            {
                Debug.LogError("Socket with ID " + socketId + " not found or is null.");
            }
        }
    }
}