using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector2 pointA;
    [SerializeField] private Vector2 pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 0.5f;

    private Vector2 currentTarget;
    private Vector2 startPosition;
    private float waitCounter;

    private void Start()
    {
        startPosition = transform.position;
        pointA += startPosition;
        pointB += startPosition;
        currentTarget = pointB;
        waitCounter = 0f;
    }

    private void Update()
    {
        if (waitCounter > 0)
        {
            waitCounter -= Time.deltaTime;
            return;
        }

        // Move towards the current target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        // Check if we've reached the target
        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            // Switch target and wait
            currentTarget = currentTarget == pointA ? pointB : pointA;
            waitCounter = waitTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Parent the player to the platform so they move with it
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent the player when they leave the platform
            collision.gameObject.transform.SetParent(null);
        }
    }

    // Optional: Visualize the platform's path in the editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Vector2 pos = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pos + pointA, 0.3f);
            Gizmos.DrawWireSphere(pos + pointB, 0.3f);
            Gizmos.DrawLine(pos + pointA, pos + pointB);
        }
    }
} 