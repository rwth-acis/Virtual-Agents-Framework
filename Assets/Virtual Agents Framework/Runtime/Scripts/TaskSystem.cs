using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public abstract class TaskSystem : MonoBehaviour, ITaskSystem
    {
        public abstract void UpdateTaskSystem();
    }
}
