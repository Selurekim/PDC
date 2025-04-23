using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private bool facingRight = true;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Transform player;
    private Rigidbody2D rb;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private Vector2 movement = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned to " + gameObject.name);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Check if player is in detection range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isChasing = distanceToPlayer <= detectionRange;

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Update facing direction
        if (movement.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (movement.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Apply movement
        float currentSpeed = isChasing ? chaseSpeed : moveSpeed;
        rb.velocity = new Vector2(movement.x * currentSpeed, rb.velocity.y);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 targetPosition = patrolPoints[currentPatrolIndex].position;
        float distance = Vector2.Distance(transform.position, targetPosition);

        // Move towards patrol point
        movement = (targetPosition - (Vector2)transform.position).normalized;

        // Check if we've reached the current patrol point
        if (distance < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        // Move towards player
        movement = (player.position - transform.position).normalized;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Draw patrol path
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
            if (patrolPoints.Length > 1 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
            }
        }
    }
} 