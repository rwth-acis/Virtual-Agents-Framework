using i5.VirtualAgents;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class WaitSampleController : MonoBehaviour
    {
        public Agent agent;
        public Transform[] waypoints;

        void Start()
        {
            agent.Tasks.GoTo(waypoints[0]);
            agent.Tasks.WaitForSeconds(2f);
            agent.Tasks.GoTo(waypoints[1]);
        }
    }
}