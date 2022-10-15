using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    // controls the gate the example and moves it up after some time
    public class GateController : MonoBehaviour
    {
        public float WaitTime;
        private Vector3 startPos;
        
        void Start()
        {
            startPos = transform.position;
            StartCoroutine(MoveUp(WaitTime));
        }

        IEnumerator MoveUp(float waittime)
        {
            yield return new WaitForSeconds(waittime);
            while (true)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, startPos.y + 3.5f, Time.deltaTime), transform.position.z);
                yield return null;
            }
        }
    }
}
