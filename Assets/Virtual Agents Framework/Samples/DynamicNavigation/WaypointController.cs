using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
	// controls the waypoint movement
	public class WaypointController : MonoBehaviour
    {
        /// <summary>
        /// The time to wait between movements in seconds.
        /// </summary>
        [Tooltip("The time to wait between movements in seconds.")]
        [SerializeField] public float waitTime;

        /// <summary>
        /// The distance to move left and right from the start position.
        /// </summary>
        [Tooltip("The distance to move left and right from the start position.")]
        [SerializeField] public float moveDistance = 3.5f; // Distance to move left and right from the start position
        private Vector3 startPos;

        public bool moveBack = true;
        public bool moveLeft = true;
        public bool moveRight = true;
        public bool moveForward = true;

        private void Start()
        {
            startPos = transform.position;

            StartCoroutine(MoveLoop(waitTime));
        }

        private IEnumerator MoveLoop(float waittime)
        {
            while (true)
            {
                if (moveLeft)
                {
                    yield return StartCoroutine(MoveLeft());
                    yield return new WaitForSeconds(waittime);
                }

                if (moveForward)
                {
                    yield return StartCoroutine(MoveForward());
                    yield return new WaitForSeconds(waittime);
                }

                if (moveRight)
                {
                    yield return StartCoroutine(MoveRight());
                    yield return new WaitForSeconds(waittime);
                }

                if (moveBack)
                {
                    yield return StartCoroutine(MoveBackward());
                    yield return new WaitForSeconds(waittime);
                }
            }
        }

        private IEnumerator MoveLeft()
        {
            Vector3 targetPosition = new Vector3(startPos.x - moveDistance, transform.position.y, transform.position.z);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveRight()
        {
            Vector3 targetPosition = new Vector3(startPos.x + moveDistance, transform.position.y, transform.position.z);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveForward()
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startPos.z + moveDistance);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveBackward()
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startPos.z - moveDistance);
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
