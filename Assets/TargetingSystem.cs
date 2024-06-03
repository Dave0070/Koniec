using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetingSystem : MonoBehaviour
{
    public GameObject enemyShip; // Drag the enemy ship object here
    public Image targetImage; // Drag the target image object here

    public float minTimeToTarget = 0.1f; // the minimum value for timeToTarget
    public float aimDistance = 50f; // the distance from the enemy ship to aim at

    private Vector3 enemyPos;
    private Vector3 playerPos;
    private Vector3 enemyVel;
    private Vector3 playerVel;
    private Vector3 targetPoint;

    void Update()
    {
        enemyPos = enemyShip.transform.position;
        playerPos = transform.position;
        enemyVel = enemyShip.GetComponent<Rigidbody>().velocity;
        playerVel = GetComponent<Rigidbody>().velocity;

        // Calculate the relative velocity of the player and enemy ships
        Vector3 relVel = playerVel - enemyVel;

        // Calculate the direction vectors for the player and enemy ships
        Vector3 enemyDir = (enemyPos - playerPos).normalized;
        Vector3 playerDir = transform.forward.normalized; // use the player's forward direction instead of its position

        // Calculate the time it will take for the player to reach the enemy's position
        float timeToTarget = 0f;
        if (relVel.magnitude > 0f)
        {
            float prod1 = Vector3.Dot(relVel, enemyDir);
            float prod2 = Vector3.Dot(relVel, playerDir);
            if (prod1 > 0f && prod2 < 0f)
            {
                timeToTarget = -Vector3.Dot(enemyDir, playerPos - enemyPos) / relVel.magnitude;
            }
        }

        // Set a minimum value for timeToTarget
        if (timeToTarget < minTimeToTarget)
        {
            timeToTarget = minTimeToTarget;
        }

        // Calculate the target point as the enemy's position plus its velocity multiplied by the time to target,
        // plus the aimDistance vector in the direction of the enemy's velocity
        if (timeToTarget > 0f)
        {
            targetPoint = enemyPos + enemyVel * timeToTarget + aimDistance * enemyVel.normalized;

            // Project the target point onto the canvas
            Canvas canvas = targetImage.GetComponentInParent<Canvas>();
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(targetPoint);
            Vector2 canvasPoint = new Vector2(screenPoint.x * canvas.pixelRect.width, screenPoint.y * canvas.pixelRect.height);
            Vector2 targetPointCanvas;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, canvasPoint, canvas.worldCamera, out targetPointCanvas);
            targetPoint = canvas.transform.TransformPoint(targetPointCanvas);

            // Set the target point as the position of the target image
            targetImage.rectTransform.position = targetPoint;
        }
    }
}