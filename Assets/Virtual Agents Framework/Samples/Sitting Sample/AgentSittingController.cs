// ...existing code...
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    /// <summary>
    /// Demonstrates different sitting behaviors using the Virtual Agents Framework.
    /// This includes sitting/standing on static chairs, stools, and interacting with mobile seats
    /// (wheelchair and skateboard) by parenting them to the agent and modifying movement parameters.
    /// </summary>
    public class AgentSittingController : SampleScheduleController
    {
        /// <summary>
        /// The static chair used in the first example for explicit sitting and standing tasks.
        /// </summary>
        [Tooltip("The static chair used in the first example for explicit sitting and standing tasks.")]
        [SerializeField] Chair Chair = null;

        /// <summary>
        /// The stool used in the second example to demonstrate automatic toggling of the sitting state.
        /// </summary>
        [Tooltip("The stool used in the second example to demonstrate automatic toggling of the sitting state.")]
        [SerializeField] Chair Stool = null;

        /// <summary>
        /// The wheelchair used in the third example to show how a mobile seat can be parented to the agent and animated.
        /// </summary>
        [Tooltip("The wheelchair used in the third example to show how a mobile seat can be parented to the agent and animated.")]
        [SerializeField] Chair Wheelchair = null;

        /// <summary>
        /// The skateboard used in the fourth example to show altering agent movement properties while on a moving seat.
        /// </summary>
        [Tooltip("The skateboard used in the fourth example to show altering agent movement properties while on a moving seat.")]
        [SerializeField] Chair Skateboard = null;

        /// <summary>
        /// The item to be picked up by the agent while riding the skateboard.
        /// </summary>
        [Tooltip("The item to be picked up by the agent while riding the skateboard.")]
        [SerializeField] Item Item = null;
        
        /// <summary>
        /// The target waypoint the agent travels to while seated in the wheelchair.
        /// </summary>
        [Tooltip("The target waypoint the agent travels to while seated in the wheelchair.")]
        [SerializeField] Transform waypoint1 = null;

        /// <summary>
        /// The target waypoint the agent travels to while riding the skateboard.
        /// </summary>
        [Tooltip("The target waypoint the agent travels to while riding the skateboard.")]
        [SerializeField] Transform waypoint2 = null;
        
        protected override void Start()
        {
            base.Start();
            
            
            // First example: Go to and sit on a chair, wait for 3 seconds, and then stand up.
            taskSystem.Tasks.GoToAndSit(Chair);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.StandUp(Chair);
            
            // Second example: Go to and sit on a stool, wait for 3 seconds, and then stand up.
            taskSystem.Tasks.GoToAndSit(Stool);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.StandUp(Stool);
            
            // Third example: Sit on a wheelchair, move to waypoint 1 while "riding" it, and then stand up.
            // Start the GoToAndSit task for the wheelchair and cast it to a TaskBundle to listen to its events.
            TaskBundle wheelChairTask = (TaskBundle) taskSystem.Tasks.GoToAndSit(Wheelchair);
            
            // Once the sitting task finishes (the agent is seated), parent the wheelchair to the agent
            // and connect the tire animation script to the agent's NavMeshAgent.
            wheelChairTask.OnTaskFinished += () =>
            {
                Wheelchair.transform.parent = agent.transform;
                Wheelchair.GetComponent<TireAnimation>().agent = agent.GetComponent<NavMeshAgent>();
            };
            
            // Instruct the agent to move to the first waypoint (the wheelchair will move with the agent).
            taskSystem.Tasks.GoTo(waypoint1);
            
            // Stand up from/leave the wheelchair once the destination is reached.
            TaskBundle wheelChairTaskEnd = (TaskBundle) taskSystem.Tasks.GoToAndSit(Wheelchair);
            
            // Once the stand up task finishes, deparent the wheelchair and stop animating its tires.
            wheelChairTaskEnd.OnTaskFinished += () =>
            {
                Wheelchair.transform.parent = null;
                Wheelchair.GetComponent<TireAnimation>().agent = null;
            };
            
            // Fourth example: Ride a skateboard to pick up an item, travel to waypoint 2, and then leave the skateboard.
            // This example shows some of the limitations of using the SittingTask for "vehicles".
            TaskBundle skateBoardTask = (TaskBundle) taskSystem.Tasks.GoToAndSit(Skateboard);
            
            // Once seated, parent the skateboard, set up tire animation, and adjust agent velocity/rotation parameters
            // to mimic a skateboard's dynamic movement (faster speed/acceleration, slower rotation).
            skateBoardTask.OnTaskFinished += () =>
            {
                Skateboard.transform.parent = agent.transform;
                Skateboard.GetComponent<TireAnimation>().agent = agent.GetComponent<NavMeshAgent>();
                
                // Decrease rotation speed and increase acceleration to match skateboard movement style more closely
                agent.GetComponent<NavMeshAgent>().angularSpeed /= 6;
                agent.GetComponent<NavMeshAgent>().speed *= 3;
                agent.GetComponent<NavMeshAgent>().acceleration *= 3;

                // Note: The NavMeshAgent is not meant to be used for vehicles with steering physics, 
                // but it works well enough for a simple demonstration and prototyping.
            };
            
            // Pick up the item while riding.
            taskSystem.Tasks.GoToAndPickUp(Item.gameObject);
            
            // Go to the second waypoint on the skateboard.
            taskSystem.Tasks.GoTo(waypoint2);
            
            // Stand up from/leave the skateboard.
            TaskBundle skateBoardTaskEnd = (TaskBundle) taskSystem.Tasks.GoToAndSit(Skateboard);
            
            // Deparent the skateboard and disable tire animation when the task finishes.
            skateBoardTaskEnd.OnTaskFinished += () =>
            {
                Skateboard.transform.parent = null;
                Skateboard.GetComponent<TireAnimation>().agent = null;
                // Revert back to original NavMeshAgent parameters
                agent.GetComponent<NavMeshAgent>().angularSpeed *= 6;
                agent.GetComponent<NavMeshAgent>().speed /= 3;
                agent.GetComponent<NavMeshAgent>().acceleration /= 3;   // Revert back to original NavMeshAgent parameters
            };
        }
    }
}