using i5.VirtualAgents.AgentTasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    //Item importes importance from 
    public class Item : PossibleLookAtTarget
    {

        private bool isPickedUp = false;
        public bool canBePickedUp = false;
        

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
        public void setIsPickedUp(bool pickedUp)
        {
            this.isPickedUp = pickedUp;
            if(this.isPickedUp)
            {
                canCurrentlyBeLookedAt = false;
            }
            else
            {
                canCurrentlyBeLookedAt = true;
            }
        }
        public bool getIsPickedUp()
        {
            return this.isPickedUp;
        }
    }
}
