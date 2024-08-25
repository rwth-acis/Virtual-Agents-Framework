using System.Collections;
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
            // Test audio reading out documentation entry
            AgentAudioTask audioTask1 = new AgentAudioTask(audioClip[0]);
            // Soundeffect
            AgentAudioTask audioTask2 = new AgentAudioTask(audioClip[1]);
            taskSystem.ScheduleTask(audioTask1);
            StartCoroutine(PauseAndResumeAudio(audioTask1));
            taskSystem.ScheduleTask(audioTask2);
        }
        private IEnumerator PauseAndResumeAudio(AgentAudioTask audioTask)
        {
            yield return new WaitForSeconds(11);
            // Pause Audio
            audioTask.PauseAudio();
            yield return new WaitForSeconds(2);
            // Continue Audio
            audioTask.ContinueAudio();
        }
    }
}