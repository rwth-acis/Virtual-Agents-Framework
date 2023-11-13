using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    // controls the waypoint movement
    public class ItemController : MonoBehaviour
    {

        public float WaitTime;
        public float MoveDistance = 3.5f; // Distance to move left and right from the start position
        private Item item;
        private Vector3 startPos;

        private int CurrentMovement = 0;

        private void Start()
        {
            item = GetComponent<Item>();
            startPos = transform.position;
            item.dropEvent.AddListener(ItemWasDropped);
            StartCoroutine(MoveLoop(WaitTime));
        }

        private void ItemWasDropped()
        {
            //If item has a rigitbody component, activate it
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        private IEnumerator MoveLoop(float waittime)
        {
            while (!item.GetIsPickedUp())
            {
                yield return MoveInSquare();
                yield return new WaitForSeconds(waittime);

            }
        }

        private IEnumerator MoveInSquare()
        {
            switch (CurrentMovement)
            {
                case 0:
                    yield return StartCoroutine(MoveLeft());
                    CurrentMovement++;
                    break;
                case 1:
                    yield return StartCoroutine(MoveForward());
                    CurrentMovement++;
                    break;
                case 2:
                    yield return StartCoroutine(MoveRight());
                    CurrentMovement++;
                    break;
                case 3:
                    yield return StartCoroutine(MoveBackward());
                    CurrentMovement = 0;
                    break;
            }
        }

        private IEnumerator MoveLeft()
        {
            Vector3 targetPosition = new Vector3(startPos.x - MoveDistance, transform.position.y, transform.position.z);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveRight()
        {
            Vector3 targetPosition = new Vector3(startPos.x + MoveDistance, transform.position.y, transform.position.z);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveForward()
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startPos.z + MoveDistance);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveBackward()
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startPos.z - MoveDistance);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator PerformMovement(Vector3 targetPosition)
        {
			float t = 0;

			while (t < 1)
			{
                if (item.GetIsPickedUp()) break;
                t += Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, targetPosition, t);
				yield return null;
			}

			// Ensure that the object reaches exactly the target position
			transform.position = targetPosition;
		}
    }
}
