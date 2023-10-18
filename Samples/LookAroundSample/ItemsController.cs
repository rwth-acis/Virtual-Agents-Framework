using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    // controls the waypoint movement
    public class ItemsController : MonoBehaviour
    {

        public float WaitTime;
        public float MoveDistance = 3.5f; // Distance to move left and right from the start position
        private Item item;
        private Vector3 startPos;

        private void Start()
        {
            item = GetComponent<Item>();
            startPos = transform.position;

            StartCoroutine(MoveLoop(WaitTime));
        }

        private IEnumerator MoveLoop(float waittime)
        {
            while (!item.getIsPickedUp())
            {
                yield return StartCoroutine(MoveLeft());
                yield return new WaitForSeconds(waittime);
                if (item.getIsPickedUp()) break;

                yield return StartCoroutine(MoveForward());
                yield return new WaitForSeconds(waittime);
                if (item.getIsPickedUp()) break;

                yield return StartCoroutine(MoveRight());
                yield return new WaitForSeconds(waittime);
                if (item.getIsPickedUp()) break;

                yield return StartCoroutine(MoveBackward());
                yield return new WaitForSeconds(waittime);

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
				t += Time.deltaTime;
				transform.position = Vector3.Lerp(transform.position, targetPosition, t);
				yield return null;
			}

			// Ensure that the object reaches exactly the target position
			transform.position = targetPosition;
		}
    }
}
