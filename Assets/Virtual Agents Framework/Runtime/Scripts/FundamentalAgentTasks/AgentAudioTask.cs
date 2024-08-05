using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
	public class AgentAudioTask : AgentBaseTask, ISerializable
	{
		/// <summary>
		/// The audio to be played
		/// </summary>
		public AudioClip Audio;

		/// <summary>
		/// The audio source which plays the audio
		/// </summary>
		public AudioSource AgentAudioSource;

		/// <summary>
		/// Creates a new audio task
		/// </summary>
		/// <param name="audio">The audio to be played</param>
		public AgentAudioTask(AudioClip audio, int priority = 0)
		{
			Audio = audio;
		}

		/// <summary>
		/// Starts the audio task
		/// </summary>
		/// <param name="agent">The agent which should execute the movement task</param>
		public override void StartExecution(Agent agent)
		{
			base.StartExecution(agent);
			AgentAudioSource = agent.GetComponent<AudioSource>();
			AgentAudioSource.clip = Audio;
			AgentAudioSource.Play();
		}

		/// <summary>
		/// Finish the task
		/// </summary>
		public override void StopExecution()
		{
			base.StopExecution();
			AgentAudioSource.Stop();
		}

		public void Serialize(SerializationDataContainer serializer)
		{
		}

		public void Deserialize(SerializationDataContainer serializer)
		{
		}
	}
}