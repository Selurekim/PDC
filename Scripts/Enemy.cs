using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionChangeChance = 0.01f;
    [SerializeField] private float jumpChance = 0.001f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Vector2 movementBounds = new Vector2(-5f, 5f);

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int moveDirection = 1;
    private bool isDead;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isDead) return;

        // Random direction change
        if (Random.value < directionChangeChance)
        {
            moveDirection *= -1;
        }

        // Random jump
        if (Random.value < jumpChance)
        {
            Jump();
        }

        // Check movement bounds
        float newX = transform.position.x + (moveSpeed * moveDirection * Time.deltaTime);
        if (newX < startPosition.x + movementBounds.x || newX > startPosition.x + movementBounds.y)
        {
            moveDirection *= -1;
            newX = transform.position.x;
        }

        // Move
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Update sprite direction
        spriteRenderer.flipX = moveDirection < 0;
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void Die()
    {
        isDead = true;
        rb.gravityScale = 1;
        rb.constraints = RigidbodyConstraints2D.None;
        transform.rotation = Quaternion.Euler(0, 0, 180); // Flip upside down
        GetComponent<Collider2D>().enabled = false;
        
        // Destroy when off screen
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Destroy(gameObject);
        }
    }
} 