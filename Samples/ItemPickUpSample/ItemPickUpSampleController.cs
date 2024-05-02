using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class ItemPickUpSampleController : SampleScheduleController
    {
        [SerializeField] private List<GameObject> pickUpObjects;

        [SerializeField] private Transform dropFirstItemHere;

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
                    // TODO This does not work with the TaskBundle TaskActions
                    //taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default, MeshSockets.SocketId.RightLowerArm);
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