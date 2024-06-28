using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Socket on the agent to which items can be attached, e.g., for hands or item belts on the agent
    /// </summary>
	public class MeshSocket : MonoBehaviour
    {
        /// <summary>
        /// The id of the socket
        /// </summary>
        [field: Tooltip("The id of the socket")]
        [field: SerializeField]
        public MeshSockets.SocketId SocketId { get; private set; }

        private Dictionary<Item, Transform> offsetMap = new();

        /// <summary>
        /// Attaches an item to the mesh socket based on the definition which is encoded in hte item
        /// </summary>
        /// <param name="item">The item to attach to the socket</param>
        public void Attach(Item item)
        {
            // Create new offset for the new item
            Transform attachPointOffset = new GameObject().transform;
            attachPointOffset.name = "AttachPointOffsetBasedOnGrabTargetOf" + item.name;
            // Add rigTransform component to the offset object
            RigTransform rigTransform = attachPointOffset.gameObject.AddComponent<RigTransform>();

            offsetMap[item] = attachPointOffset;
            attachPointOffset.SetParent(this.transform);
            // item.grapTarget should be at the same position as the attachPoint
            item.transform.SetParent(attachPointOffset, false);
            item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Calculate the diffrerence between the attachPoint and the item's grapTarget
            Vector3 offsetPosition = item.GrabTarget.transform.position - attachPointOffset.position;
            Quaternion offsetRotation = Quaternion.Inverse(attachPointOffset.transform.rotation) * item.GrabTarget.transform.rotation;
            // Move the attachPointOffset so that the attachPoint allignes with the attachPoint
            attachPointOffset.transform.SetLocalPositionAndRotation(offsetPosition, offsetRotation);

            StartCoroutine(StopFurtherMovement(item.transform));
        }

        /// <summary>
        /// Detaches the given item from the socket
        /// </summary>
        /// <param name="item">Detaches the item from the socket</param>
        public void Detach(Item item)
        {
            // Destroy the offset
            Destroy(offsetMap[item].gameObject);
            offsetMap.Remove(item);
            item.transform.SetParent(null, true);
            item.IsDropped();
        }

        // In case there is any movement that hasn't stopped yet, local position and rotation will be set to zero as long as the object is moving
        private IEnumerator StopFurtherMovement(Transform objectTransform)
        {
            //a wait time of 0.025 seconds is used to avoid edge cases where the object doesn't move on every frame or not on the same frame as the coroutine is executed
            yield return new WaitForSeconds(0.025f);
            while (objectTransform.localPosition != Vector3.zero)
            {
                objectTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}