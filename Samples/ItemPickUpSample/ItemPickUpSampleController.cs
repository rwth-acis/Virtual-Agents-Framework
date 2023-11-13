using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class ItemPickUpSampleController : SampleScheduleController
    {
        public List<GameObject> pickUpObjects;

        protected override void Start()
        {
            base.Start();
            
            // add a task to go to the first object and pick it up, then go to the second object and pick it up with the left hand
            for (int i = 0; i < pickUpObjects.Count; i++)
            {
                if (i % 2 != 1)
                {
                    taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default);
                }
                else
                    taskSystem.Tasks.GoToAndPickUp(pickUpObjects[i], default, MeshSockets.SocketId.LeftHand);
            }

            // add a task to go to the start an drop the first item
            taskSystem.Tasks.GoToAndDropItem(Vector3.zero, pickUpObjects[0]);

            // add a task to move a bit and then drop all item
            taskSystem.Tasks.GoToAndDropItem(new Vector3(10, 0, 0));

            //move away from the items
            taskSystem.Tasks.GoTo(new Vector3(5, 0, 0));
        }
    }
}