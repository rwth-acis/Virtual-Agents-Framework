using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public class Item : MonoBehaviour
    {
        public bool isPickedUp { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            isPickedUp = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
