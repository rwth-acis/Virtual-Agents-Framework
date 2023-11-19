using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace i5.VirtualAgents
{
	/// <summary>
	/// Represents an item which can be picked up by an agent
	/// </summary>
	public class Item : MonoBehaviour
	{
		private bool isPickedUp = false;

		public bool IsPickedUp
		{
			get => isPickedUp;
			set
			{
				isPickedUp = value;

				if (adaptiveGazeTarget)
				{
					if (this.isPickedUp)
					{
						adaptiveGazeTarget.canCurrentlyBeLookedAt = false;
					}
					else
					{
						adaptiveGazeTarget.canCurrentlyBeLookedAt = true;
					}
				}
			}
		}

		[field: SerializeField]
		public bool CanBePickedUp { get; set; } = false;

		/// <summary>
		/// This event can be listend to, to get notified when the item is dropped
		/// </summary>
		public UnityEvent dropEvent = new();

		/// <summary>
		/// grab is where IK of the Hand will be applied to, for example a handle of a cup. Initially it is the same as the object itself.
		/// </summary>
		[field: Tooltip("grab is where IK of the Hand will be applied to, for example a handle of a cup. Initially it is the same as the object itself.")]
		[field: SerializeField]
		public Transform GrabTarget { get; private set; }

		private AdaptiveGazeTarget adaptiveGazeTarget;

		private void Start()
		{
			if (GrabTarget == null)
			{
				GrabTarget = transform;
			}

			isPickedUp = false;

			adaptiveGazeTarget = GetComponent<AdaptiveGazeTarget>();
		}

		public void IsDropped()
		{
			IsPickedUp = false;
			dropEvent.Invoke();
		}
	}
}
