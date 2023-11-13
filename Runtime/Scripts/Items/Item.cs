using UnityEngine;
using UnityEngine.Events;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Represents an item which can be picked up by an agent
    /// </summary>
    public class Item : MonoBehaviour
    {

        private bool isPickedUp = false;
        public bool canBePickedUp = false;

        /// <summary>
        /// This event can be listend to, to get notified when the item is dropped
        /// </summary>
        public UnityEvent dropEvent = new();

        /// <summary>
        /// grap is where IK of the Hand will be applied to, for example a handle of a cup. Initially it is the same as the object itself
        /// </summary>
        public Transform grapTarget;

        private AdaptiveGazeTarget adaptiveGazeTarget;

        void Start()
        {

            if (grapTarget == null)
            {
                grapTarget = transform;
            }

            isPickedUp = false;

            adaptiveGazeTarget = GetComponent<AdaptiveGazeTarget>();
        }
        public void SetIsPickedUp(bool pickedUp)
        {
            this.isPickedUp = pickedUp;

            if (adaptiveGazeTarget)
            {
                if (this.isPickedUp)
                {
                    adaptiveGazeTarget.canCurrentlyBeLookedAt = false;
                }
                else
                {
                    adaptiveGazeTarget.canCurrentlyBeLookedAt = true;
                }
            }

        }
        public bool GetIsPickedUp()
        {
            return this.isPickedUp;
        }

        public void IsDropped()
        {
            SetIsPickedUp(false);
            dropEvent.Invoke();
        }
    }
}
