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
            AgentAudioTask audioTask = new AgentAudioTask(audioClip[0]);
            taskSystem.ScheduleTask(audioTask);
            StartCoroutine(PauseAndResumeAudio(audioTask));
        }
        private IEnumerator PauseAndResumeAudio(AgentAudioTask audioTask)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("Pause Audio");
            audioTask.PauseAudio();
            yield return new WaitForSeconds(2);
            Debug.Log("Continue Audio");
            audioTask.ContinueAudio();
        }
    }
}