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
        // TODO: we should make this class extendable by more sockets than the pre-defined IDs
        public enum SocketId
        {
            Spine,
            RightHand,
            LeftHand
        }

        /// <summary>
        /// Define the two bone IK constraints for the arms that is uses in the AgetPickUpTask
        /// </summary>
        [field: SerializeField]
        public TwoBoneIKConstraint TwoBoneIKConstraintRightArm { get; private set; }
        [field: SerializeField]
        public TwoBoneIKConstraint TwoBoneIKConstraintLeftArm { get; private set; }

        Dictionary<SocketId, MeshSocket> socketMap = new();

        // Initializes the component and collects the MeshSockets
        private void Start()
        {
            // Collect all the MeshSockets in the agent
            MeshSocket[] sockets = GetComponentsInChildren<MeshSocket>();
            foreach (MeshSocket socket in sockets)
            {
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
            // TODO: null check
            socketMap[socketId].Attach(item);
        }

        /// <summary>
        /// Detaches an item from its socket
        /// </summary>
        /// <param name="item">The item to detach</param>
        public void Detach(Item item)
        {
            // Get the socketId of the socket that the item is attached to
            // TODO: maybe we could store the MeshSocket to which an item is currently attached as this would be more stable against changes in the hierarchy
            SocketId socketId = item.transform.parent.parent.GetComponent<MeshSocket>().SocketId;

            socketMap[socketId].Detach(item);
        }
    }
}