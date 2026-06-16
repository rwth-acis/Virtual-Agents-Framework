// ...existing code...
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentSittingController : SampleScheduleController
    {
        [SerializeField] Chair Chair = null;
        [SerializeField] Chair Stool = null;
        [SerializeField] GameObject Item = null;
        
        
        protected override void Start()
        {
            base.Start();
            
            taskSystem.Tasks.GoToAndSit(Chair, SittingDirection.SITDOWN);
            taskSystem.Tasks.WaitForSeconds(3);
            // GoToAndSit is also used for making the agent standup from a chair
            taskSystem.Tasks.GoToAndSit(Chair, SittingDirection.STANDUP);
            
            taskSystem.Tasks.GoToAndPickUp(Item);
            
            // If no SittingDirection is defined it will automatically be toggled
            taskSystem.Tasks.GoToAndSit(Stool);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.GoToAndSit(Stool);
        }

    }
}