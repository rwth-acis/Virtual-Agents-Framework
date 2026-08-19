using UnityEngine;
using System.Collections;

namespace i5.VirtualAgents.Examples
{
    public class Button : MonoBehaviour
    {
        public float waitBeforeClick = 0.65f;
        public void startPressed()
        {
            Debug.Log("startPressed");
            StartCoroutine(pressed());
        }
        
        /// <summary>
        /// When method is called object moves down for a second and then up again. The movement is in an S-curve
        /// </summary>
        public IEnumerator pressed()
        {
            // Wait for the specified time
            float elapsed = 0f;
            while (elapsed < waitBeforeClick)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Vector3 startPos = transform.localPosition;
            Vector3 downPos = startPos + Vector3.down * 0.5f;
            float duration = 1f;
            float halfDuration = duration / 2f;
            
            // Move down with S-curve (first 0.5 seconds)
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                float eased = Mathf.SmoothStep(0f, 1f, t);
                transform.localPosition = Vector3.Lerp(startPos, downPos, eased);
                yield return null;
            }
            
            // Move back up with S-curve (next 0.5 seconds)
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                float eased = Mathf.SmoothStep(0f, 1f, t);
                transform.localPosition = Vector3.Lerp(downPos, startPos, eased);
               yield return null;
            }
            
            transform.localPosition = startPos;
        }
    }
}
