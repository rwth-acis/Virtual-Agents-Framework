using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class GateController : MonoBehaviour
    {
        public float WaitTime;
        private Vector3 startPos;
        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position;
            StartCoroutine(MoveUp(WaitTime));
        }

        // Update is called once per frame
        void Update()
        {
            
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
