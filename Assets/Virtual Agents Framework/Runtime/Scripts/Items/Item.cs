using UnityEngine;

namespace i5.VirtualAgents
{
    //Item importes importance from 
    public class Item : MonoBehaviour
    {

        private bool isPickedUp = false;
        public bool canBePickedUp = false;


        /// <summary>
        /// Grip is where IK of the Hand will be applied to, for example a handle of a cup. Initially it is the same as the object itself
        /// </summary>
        public Transform gripTarget;

        private PossibleLookAtTarget possibleLookAtTargetScript;

        void Start()
        {
            if (gripTarget == null)
            {
                gripTarget = transform;
            }

            isPickedUp = false;

            possibleLookAtTargetScript = GetComponent<PossibleLookAtTarget>();
        }
        public void SetIsPickedUp(bool pickedUp)
        {
            this.isPickedUp = pickedUp;

            if (possibleLookAtTargetScript)
            {
                if (this.isPickedUp)
                {
                    possibleLookAtTargetScript.canCurrentlyBeLookedAt = false;
                }
                else
                {
                    possibleLookAtTargetScript.canCurrentlyBeLookedAt = true;
                }
            }

        }
        public bool GetIsPickedUp()
        {
            return this.isPickedUp;
        }
    }
}
