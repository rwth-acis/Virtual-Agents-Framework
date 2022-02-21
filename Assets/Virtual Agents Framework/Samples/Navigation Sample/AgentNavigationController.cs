using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents;

namespace i5.VirtualAgents.Examples
{
    public class AgentNavigationController : MonoBehaviour
    {
        public Agent agent;
        public List<Transform> waypoints;

        void Start()
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                agent.Tasks.WalkTo(waypoints[i].position);
            }
        }
    }
}