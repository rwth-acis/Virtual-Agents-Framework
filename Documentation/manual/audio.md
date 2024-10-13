# Audio Tasks
`AgentAudioTask` is a task that plays an audio clip. It can be used to add sound effects or speech to the agent's actions.
The `PauseAudio()` and `ContinueAudio()` functions can be used to pause and continue the audio playback.
One can use the SpatialBlend property of the AudioSource component to adjust how much differently the audio sounds, depending on angle and distance towards the agent, meaning how "3D" it sounds.
When set to 0, the audio sounds the same, regardless of camera position. When set to 1, it can only be heard when near and directly facing the agent.

# Construct an Audio Task
For an audio task the following prerequisites are necessary:
- The agent needs an AudioSource component. This should already be set up in the agent prefab.
- An audio file to be played. You can use all file types supported by Unity's Audio Clip type, see [here](https://docs.unity3d.com/Manual/class-AudioClip.html).
- To receive the audio, you need a GameObject with an AudioListener component, typically the camera. 

Then simply create an AgentAudioTask object and pass the audio clip to its constructor.

Example:
```csharp
// Add your audio file in the editor
[SerializeField] private AudioClip audioClip;

public void Start()
        {
            AgentAudioTask audioTask = new AgentAudioTask(audioClip);
            // The audio should play as soon as the scheduler starts the task
            taskSystem.ScheduleTask(audioTask);
            StartCoroutine(PauseAndResumeAudio(audioTask));
        }
        // Pause and resume the audio after a few seconds
        private IEnumerator PauseAndResumeAudio(AgentAudioTask audioTask)
        {
            yield return new WaitForSeconds(5);
            // Pause Audio
            audioTask.PauseAudio();
            yield return new WaitForSeconds(2);
            // Continue Audio
            audioTask.ContinueAudio();
        }
```


# Example Scene

In the provided `Audio Sample` scene, the agent plays a recording of the documentation page, pausing for a short time while doing so.
Then a short sound effect follows. 