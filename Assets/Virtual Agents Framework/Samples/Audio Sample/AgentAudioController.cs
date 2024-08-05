using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AgentAudioController : SampleScheduleController
    {
        [SerializeField] private List<AudioClip> audioClip;
        protected override void Start()
        {
            base.Start();
            AgentAudioTask audioTask = new AgentAudioTask(audioClip[0]);
            taskSystem.ScheduleTask(audioTask);
        }
    }
}