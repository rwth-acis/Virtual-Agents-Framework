// ...existing code...
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentSittingController : SampleScheduleController
    {
        [SerializeField] GameObject Chair = null;
        [SerializeField] GameObject Stool = null;
        protected override void Start()
        {
            base.Start();
            
            taskSystem.Tasks.GoToAndSit(Chair, SittingDirection.SITDOWN);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.GoToAndSit(Chair, SittingDirection.STANDUP);
            
            taskSystem.Tasks.GoToAndSit(Stool);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.GoToAndSit(Stool);
        }

    }
}