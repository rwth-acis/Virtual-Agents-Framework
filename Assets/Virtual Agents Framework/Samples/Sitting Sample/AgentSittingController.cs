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
            
            taskSystem.Tasks.GoToAndSit(Chair);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.StandUp(Chair);
            
            taskSystem.Tasks.GoToAndPickUp(Item);
            
            taskSystem.Tasks.GoToAndSit(Stool);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.StandUp(Stool);
        }

    }
}