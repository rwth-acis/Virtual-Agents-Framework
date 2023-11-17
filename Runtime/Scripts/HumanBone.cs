using System;
using UnityEngine;

namespace i5.VirtualAgents
{
	[System.Serializable]
	public class HumanBone
	{
		public HumanBodyBones bone;
		[Range(0, 1)]
		public float weight;
	}

}