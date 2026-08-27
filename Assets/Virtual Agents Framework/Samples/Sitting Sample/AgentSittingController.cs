using i5.VirtualAgents.AgentTasks;
using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents.Examples
{
    /// <summary>
    /// Demonstrates different sitting behaviors using the Virtual Agents Framework.
    /// This includes sitting/standing on static chairs, stools, and interacting with mobile seats
    /// (wheelchair and skateboard) by parenting them to the agent, modifying movement parameters,
    /// and switching the NavMesh agent type for the wheelchair to use a larger radius.
    /// </summary>
    public class AgentSittingController : SampleScheduleController
    {
        /// <summary>
        /// The static chair used in the first example for explicit sitting and standing tasks.
        /// </summary>
        [Tooltip("The static chair used in the first example for explicit sitting and standing tasks.")]
        [SerializeField]
        private Chair chair;

        /// <summary>
        /// The stool used in the second example to demonstrate automatic toggling of the sitting state.
        /// </summary>
        [Tooltip("The stool used in the second example to demonstrate automatic toggling of the sitting state.")]
        [SerializeField]
        private Chair stool;

        /// <summary>
        /// The wheelchair used in the third example to show how a mobile seat can be parented to the agent and animated.
        /// </summary>
        [Tooltip("The wheelchair used in the third example to show how a mobile seat can be parented to the agent and animated.")]
        [SerializeField]
        private Chair wheelchair;

        /// <summary>
        /// The skateboard used in the fourth example to show altering agent movement properties while on a moving seat.
        /// </summary>
        [Tooltip("The skateboard used in the fourth example to show altering agent movement properties while on a moving seat.")]
        [SerializeField]
        private Chair skateboard;

        /// <summary>
        /// The item to be picked up by the agent while riding the skateboard.
        /// </summary>
        [Tooltip("The item to be picked up by the agent while riding the skateboard.")]
        [SerializeField]
        private Item item;

        /// <summary>
        /// The target waypoint the agent travels to while seated in the wheelchair.
        /// </summary>
        [Tooltip("The target waypoint the agent travels to while seated in the wheelchair.")]
        [SerializeField]
        private Transform waypoint1;

        /// <summary>
        /// The target waypoint the agent travels to while riding the skateboard.
        /// </summary>
        [Tooltip("The target waypoint the agent travels to while riding the skateboard.")]
        [SerializeField]
        private Transform waypoint2;
        
        /// <summary>
        /// The target waypoint the agent goes to after leaving the skateboard.
        /// </summary>
        [Tooltip("The target waypoint the agent goes to after leaving the skateboard.")]
        [SerializeField]
        private Transform waypoint3;
        
        /// <summary>
        /// Whether the agent should sit on the chair and stool as part of this example.
        /// </summary>
        [Tooltip("Whether the agent should sit on the chair and stool as part of this example.")]
        [SerializeField]
        private bool sitOnChairs = true;
        [SerializeField]
        private bool sitOnWheelchair = true;
        [SerializeField]
        private bool sitOnSkateboard = true;

        // Store values to restore the NavMeshAgent's original parameters after riding the skateboard.
        private float originalSpeed;
        private float originalAngularSpeed;
        private float originalAcceleration;
        private float originalStoppingDistance;


        protected override void Start()
        {
            base.Start();
            NavMeshAgent navMeshAgent = agent.GetComponent<NavMeshAgent>();

            if (sitOnChairs)
            {
                // First example: Go to and sit on a chair, wait for 3 seconds, and then stand up.
                taskSystem.Tasks.GoToAndSit(chair);
                taskSystem.Tasks.WaitForSeconds(3);
                taskSystem.Tasks.StandUp(chair);

                // Second example: Go to and sit on a stool, wait for 3 seconds, and then stand up.
                taskSystem.Tasks.GoToAndSit(stool);
                taskSystem.Tasks.WaitForSeconds(3);
                taskSystem.Tasks.StandUp(stool);
            }

            if (sitOnWheelchair){
                // Third example: Sit on a wheelchair, move to waypoint 1 while "riding" it, and then stand up.
                // Start the GoToAndSit task for the wheelchair and cast it to a TaskBundle to listen to its events.
                TaskBundle wheelChairTask = (TaskBundle)taskSystem.Tasks.GoToAndSit(wheelchair);

                // Once the sitting task finishes (the agent is seated), parent the wheelchair to the agent,
                // connect the tire animation script to the agent's NavMeshAgent,
                // and switch the NavMeshAgent's agentTypeID to the "Wheelchair" agent type (which has a larger radius).
                wheelChairTask.OnTaskFinished += () =>
                {
                    wheelchair.transform.parent = agent.transform;
                    wheelchair.GetComponent<TireAnimation>().agent = agent.GetComponent<NavMeshAgent>();
                    wheelchair.GetComponent<NavMeshObstacle>().enabled = false;
                    navMeshAgent.agentTypeID = GetAgentTypeIDByName("Wheelchair");

                };

                // Instruct the agent to move to the first waypoint (the wheelchair will move with the agent).
                taskSystem.Tasks.GoTo(waypoint1);

                // Stand up from/leave the wheelchair once the destination is reached.
                AgentBaseTask wheelChairTaskEnd = taskSystem.Tasks.StandUp(wheelchair);

                // Once the stand up task finishes, deparent the wheelchair, stop animating its tires,
                // and restore the NavMeshAgent's agentTypeID back to the "Humanoid" agent type.
                wheelChairTaskEnd.OnTaskFinished += () =>
                {
                    wheelchair.transform.parent = null;
                    wheelchair.GetComponent<TireAnimation>().agent = null;
                    wheelchair.GetComponent<NavMeshObstacle>().enabled = true;
                    navMeshAgent.agentTypeID = GetAgentTypeIDByName("Humanoid");
                };
            }
            if(sitOnSkateboard){
                // Fourth example: Ride a skateboard to pick up an item, travel to waypoint 2, and then leave the skateboard.
                // This example shows some of the limitations of using the SittingTask for "vehicles".
                AgentBaseTask skateBoardTask = taskSystem.Tasks.GoToAndSit(skateboard);
            
                // Once seated, parent the skateboard, set up tire animation, and adjust agent velocity/rotation parameters
                // to mimic a skateboard's dynamic movement (faster speed/acceleration, slower rotation).
                skateBoardTask.OnTaskFinished += () =>
                {
                    skateboard.transform.parent = agent.transform;
                    skateboard.GetComponent<TireAnimation>().agent = agent.GetComponent<NavMeshAgent>();
                
                    // Decrease rotation speed and increase acceleration to match skateboard movement style more closely
                    // Store the original NavMeshAgent parameters to revert back later
                    originalSpeed = navMeshAgent.speed;
                    originalAngularSpeed = navMeshAgent.angularSpeed;
                    originalAcceleration = navMeshAgent.acceleration;
                    originalStoppingDistance = navMeshAgent.stoppingDistance;
                    skateboard.GetComponent<NavMeshObstacle>().enabled = false;

                    navMeshAgent.speed = 4; // Increase speed
                    navMeshAgent.angularSpeed = 80; // Decrease turning speed
                    navMeshAgent.acceleration = 80; // Increase acceleration
                    navMeshAgent.stoppingDistance = 0.5f;

                    // Note: The NavMeshAgent is not meant to be used for vehicles with steering physics, 
                    // but it works well enough for a simple demonstration and prototyping.
                };
            
                // Pick up the item while riding.
                taskSystem.Tasks.GoToAndPickUp(item.gameObject);
            
                // Go to the second waypoint on the skateboard.
                taskSystem.Tasks.GoTo(waypoint2);
            
                // Stand up from/leave the skateboard.
                AgentBaseTask skateBoardTaskEnd = taskSystem.Tasks.StandUp(skateboard);
            
                // Deparent the skateboard and disable tire animation when the task finishes.
                skateBoardTaskEnd.OnTaskFinished += () =>
                {
                    skateboard.transform.parent = null;
                    skateboard.GetComponent<TireAnimation>().agent = null;
                    skateboard.GetComponent<NavMeshObstacle>().enabled = true;
                
                    // Revert back to original NavMeshAgent parameters
                    navMeshAgent.speed = originalSpeed;
                    navMeshAgent.angularSpeed = originalAngularSpeed;
                    navMeshAgent.acceleration = originalAcceleration;
                    navMeshAgent.stoppingDistance = originalStoppingDistance;
                };
                
                // Go away from the skateboard
                taskSystem.Tasks.GoTo(waypoint3);
            }
        }

        // Helper method to get the agent type ID by name. Used to switch between "Humanoid" and "Wheelchair" agent types.
        // The Wheelchair agent type has been specifically configured in the NavMesh settings for this scene to have a larger radius for navigation.
        // See Window > AI > Navigation
        private static int GetAgentTypeIDByName(string name)
        {
            int count = NavMesh.GetSettingsCount();
            for (int i = 0; i < count; i++)
            {
                int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
                if (NavMesh.GetSettingsNameFromID(id) == name)
                {
                    return id;
                }
            }
            return -1;
        }
    }
}