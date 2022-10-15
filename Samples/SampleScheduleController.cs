using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.ScheduleBasedExecution;

namespace i5.VirtualAgents.Examples
{
    public class SampleScheduleController : MonoBehaviour
    {
        public Agent agent;
        protected ScheduleBasedTaskSystem taskSystem;

        protected virtual void Start()
        {
            taskSystem = (ScheduleBasedTaskSystem)agent.TaskSystem;
        }
    }
}
