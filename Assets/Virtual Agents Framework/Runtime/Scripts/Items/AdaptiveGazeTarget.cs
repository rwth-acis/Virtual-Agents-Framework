using UnityEngine;

namespace i5.VirtualAgents
{
	public class AdaptiveGazeTarget : MonoBehaviour
    {
        /// <summary>
        /// The importance of the item for the agent. The higher the value, the more liekly it is the agent to look at it. Increases during runtime resets novelty for the agent
        /// </summary>
        [Range(1f, 10f)]
        public float importance = 1f;

        /// <summary>
        /// Can be used to switch the object's perceivability off. This is de/activated by the item component, when the item is dropped/picked up.
        /// </summary>
        public bool canCurrentlyBeLookedAt = true;
        
        private void Start()
        {
            EnsureStandardCollider();
        }

        // Ensures that the object has some form of collider. If none is present, a box collider is added.
        private void EnsureStandardCollider()
        {
			if (GetComponent<Collider>() == null)
			{
				Debug.Log("The item " + gameObject.name + " does not have a collider. To make it possible to look at it, a standard collider will be added.");
				Vector3 objectSize = transform.localScale;

				// Add a collider based on the object's size
				BoxCollider collider = gameObject.AddComponent<BoxCollider>();
				collider.size = objectSize;
				collider.center = Vector3.zero; // Set the collider's center to the object's center
			}
        }
    }
}
