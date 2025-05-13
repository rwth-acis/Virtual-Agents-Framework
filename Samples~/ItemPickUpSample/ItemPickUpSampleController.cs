using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class ItemPickUpSampleController : SampleScheduleController
    {
        /// <summary>
        /// List of objects which should be picked up.
        /// </summary>
        [Tooltip("List of objects which should be picked up.")]
        [SerializeField] private List<GameObject> pickUpObjects;

        /// <summary>
        /// The position where the first item should be dropped.
        /// </summary>
        [Tooltip("The position where the first item should be dropped.")]
        [SerializeField] private Transform dropFirstItemHere;

        /// <summary>
        /// The position where all other items should be dropped.
        /// </summary>
        [Tooltip("The position where all other items should be dropped.")]
        [SerializeField] private Transform dropAllOtherItemsHere;

        protected override void Start()
        {
            base.Start();

            // add a task to go to the first object and pick it up, then go to the second object and pick it up with the left hand

            for (int i = 0; i < pickUpObjects.Count; i++)
            {
                if(i == 0)
                {
                    taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default);
                }
                else if(i == 1)
                {
                    taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default, MeshSockets.SocketId.LeftBack);
                }
                else if (i == 2)
                {
                    taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default, MeshSockets.SocketId.RightLowerArm);
                }
                else 
                {
                    taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default, MeshSockets.SocketId.LeftHand);
                }   
            }

            // add a task to go to the start an drop the first item
            taskSystem.Tasks.GoToAndDropItem(dropFirstItemHere, pickUpObjects[0]);

            // add a task to move a bit and then drop all item
            taskSystem.Tasks.GoToAndDropItem(dropAllOtherItemsHere);

            //move away from the items
            taskSystem.Tasks.GoTo(Vector3.zero);
        }
    }
}