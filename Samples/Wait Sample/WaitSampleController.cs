using i5.VirtualAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSampleController : MonoBehaviour
{
    public Agent agent;
    public Transform[] waypoints;

    // Start is called before the first frame update
    void Start()
    {
        agent.Tasks.GoTo(waypoints[0]);
        agent.Tasks.WaitForSeconds(2f);
        agent.Tasks.GoTo(waypoints[1]);
    }
}
