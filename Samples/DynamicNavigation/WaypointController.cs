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
        [field:SerializeField] public float WaitTime {get; set;}

        /// <summary>
        /// The distance to move left and right from the start position.
        /// </summary>
        [Tooltip("The distance to move left and right from the start position.")]
        [field:SerializeField] public float MoveDistance {get; set;} = 3.5f; // Distance to move left and right from the start position
        private Vector3 startPos;

        public bool MoveBack {get; set;} = true;
        public bool MoveLeft {get; set;} = true;
        public bool MoveRight {get; set;} = true;
        public bool MoveForward {get; set;} = true;

        private void Start()
        {
            startPos = transform.position;

            StartCoroutine(MoveLoop(WaitTime));
        }

        private IEnumerator MoveLoop(float waittime)
        {
            while (true)
            {
                if (MoveLeft)
                {
                    yield return StartCoroutine(MoveToLeft());
                    yield return new WaitForSeconds(waittime);
                }

                if (MoveForward)
                {
                    yield return StartCoroutine(MoveToForward());
                    yield return new WaitForSeconds(waittime);
                }

                if (MoveRight)
                {
                    yield return StartCoroutine(MoveToRight());
                    yield return new WaitForSeconds(waittime);
                }

                if (MoveBack)
                {
                    yield return StartCoroutine(MoveToBackward());
                    yield return new WaitForSeconds(waittime);
                }
                
                if (!MoveLeft && !MoveRight && !MoveForward && !MoveBack)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator MoveToLeft()
        {
            Vector3 targetPosition = new Vector3(startPos.x - MoveDistance, transform.position.y, transform.position.z);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveToRight()
        {
            Vector3 targetPosition = new Vector3(startPos.x + MoveDistance, transform.position.y, transform.position.z);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveToForward()
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, startPos.z + MoveDistance);
            yield return PerformMovement(targetPosition);
        }

        private IEnumerator MoveToBackward()
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
