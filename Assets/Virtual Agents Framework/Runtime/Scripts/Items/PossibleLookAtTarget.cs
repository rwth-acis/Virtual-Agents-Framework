using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public class PossibleLookAtTarget : MonoBehaviour
    {
        /// <summary>
        /// The importance of the item for the agent. The higher the value, the more liekly it is the agent to look at it. Increases during runtime resets novalty for the agent
        /// </summary>
        [Range(1f, 10f)]
        public float importance = 1f;

        /// <summary>
        /// Can be used to switch the objects perceivability off. This is de/activated by the item component, when the item is dropped/picked up.
        /// </summary>
        public bool canCurrentlyBeLookedAt = true;
        
        private void Start()
        {
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                Debug.Log("The item " + gameObject.name + " does not have a collider. To make it possible to look at a standard collider will be added.");
                AddStandardCollider();
            }
        }
        private void AddStandardCollider()
        {
            if (GetComponent<Collider>() == null)
            {
                Vector3 objectSize = transform.localScale;

                // Add a collider based on the object's size
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                collider.size = objectSize;
                collider.center = Vector3.zero; // Set the collider's center to the object's center
            }
        }

    }
}
