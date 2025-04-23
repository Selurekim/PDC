using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private float minY = -5f; // Camera won't follow below this Y position

    private Vector3 velocity = Vector3.zero;
    private float initialY;

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        initialY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Don't follow below minY
        if (target.position.y < minY)
        {
            desiredPosition.y = initialY;
        }

        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / smoothSpeed
        );

        // Update position
        transform.position = smoothedPosition;
    }
} 