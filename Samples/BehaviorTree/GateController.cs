using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class GateController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(MoveUp());
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        IEnumerator MoveUp()
        {
            yield return new WaitForSeconds(3);
            while (true)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 4, Time.deltaTime), transform.position.z);
                yield return null;
            }
        }
    }
}
