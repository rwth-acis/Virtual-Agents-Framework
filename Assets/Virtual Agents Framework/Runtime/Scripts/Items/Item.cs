using i5.VirtualAgents.AgentTasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    
    public class Item : MonoBehaviour
    {

        public bool isPickedUp = false;
        public bool canBePickedUp = false;
        /// <summary>
        /// The importance of the item for the agent. The higher the value, the more liekly it is the agent to look at it. Increases during runtime resets novalty for the agent
        /// </summary>
        [Range(1f, 10f)]
        public float importance = 1f;

        /// <summary>
        /// Grip is where IK of the Hand will be applied to, for example a handle of a cup. Initially it is the same as the object itself
        /// </summary>
        public Transform gripTarget;

        void Start()
        {
            if(gripTarget == null)
            {
                gripTarget = transform;
            }
            
            isPickedUp = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
