using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class BossGoalPosition : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private Vector2 targetNormalizedPosition = new Vector2(0.5f, 0.5f); // Default position in normalized screen coordinates

        private void Start()
        {

            // Calculate target position dynamically based on screen aspect ratio
            Vector2 targetScreenPosition = CalculateTargetScreenPosition();

            // Convert target position to world space
            Vector3 targetWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, 10f));

            // Set the new position of the object
            transform.position = targetWorldPosition;
        }

        private Vector2 CalculateTargetScreenPosition()
        {
            Vector2 targetPosition;

            // Determine the smaller dimension (width or height) of the screen
            float smallerDimension = Mathf.Min(Screen.width, Screen.height);

            // Calculate the target position based on the normalized screen coordinates and the smaller dimension
            targetPosition.x = targetNormalizedPosition.x * Screen.width;
            targetPosition.y = targetNormalizedPosition.y * Screen.height;

            // Calculate the aspect ratio of the screen
            float aspectRatio = (float)Screen.width / Screen.height;

            // If the aspect ratio is less than 1, we're dealing with a portrait screen
            if (aspectRatio < 1f)
            {
                // Adjust the target position based on the smaller dimension to ensure the object appears at the correct position
                targetPosition *= aspectRatio / (16f / 9f);
            }

            return targetPosition;
        }
    }
}
