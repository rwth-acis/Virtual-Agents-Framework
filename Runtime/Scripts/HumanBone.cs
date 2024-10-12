using System;
using UnityEngine;

namespace i5.VirtualAgents
{
	[System.Serializable]
	public class HumanBone
	{
		/// <summary>
		/// The bone of the agents body
		/// </summary>
		[Tooltip("The bone of the agents body")]
		public HumanBodyBones bone;

		/// <summary>
		/// The influence weight of the bone
		/// </summary>
		[Tooltip("The influence weight of the bone")]
		[Range(0, 1)]
		public float weight;
	}
}