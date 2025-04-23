using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float movementThreshold = 0.01f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI coinText;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private Vector2 groundCheckPosition;
    private int coins;
    private bool isGameOver;
    private bool hasWon;
    private float lastMoveInput;
    private Collider2D myCollider;

    private void Start()
    {
        // Get and verify components
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on player!");
            return;
        }

        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("No Collider2D found on player!");
            return;
        }

        // Set up Rigidbody2D for platforming
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 2f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateCoinText();

        // Set up ground layer
        groundLayer = LayerMask.GetMask("Ground");
        if (groundLayer.value == 0)
        {
            Debug.LogWarning("Ground layer not found! Please create a layer named 'Ground' and assign your platforms to it.");
        }
        
        Debug.Log($"=== Player Setup ===");
        Debug.Log($"Jump Force: {jumpForce}");
        Debug.Log($"Ground Layer Mask: {groundLayer.value}");
        Debug.Log($"Ground Check Radius: {groundCheckRadius}");

        // Try to find golden platform at start to verify setup
        GameObject testPlatform = GameObject.FindGameObjectWithTag("GoldenPlatform");
        if (testPlatform == null)
        {
            Debug.LogWarning("No object with GoldenPlatform tag found in scene. Make sure to create one and tag it correctly!");
        }
    }

    private void FixedUpdate()
    {
        UpdateGroundCheck();
    }

    private void UpdateGroundCheck()
    {
        groundCheckPosition = new Vector2(transform.position.x, transform.position.y - (myCollider.bounds.extents.y));
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition, groundCheckRadius, groundLayer);
        
        // Debug ground status
        if (isGrounded)
        {
            Debug.Log("Player is grounded");
        }
    }

    private void Update()
    {
        if (isGameOver || hasWon) return;

        // Movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"=== Jump Attempt ===");
            Debug.Log($"Is Grounded: {isGrounded}");
            Debug.Log($"Ground Layer Mask: {groundLayer.value}");

            if (isGrounded)
            {
                Debug.Log("Jumping!");
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        // Animation and sprite flipping
        bool isMoving = Mathf.Abs(rb.velocity.x) > movementThreshold;
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", rb.velocity.y);
        }

        if (Mathf.Abs(moveInput) > movementThreshold && spriteRenderer != null)
        {
            spriteRenderer.flipX = moveInput < 0;
            lastMoveInput = moveInput;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only check for enemy collision if the Enemy tag exists
        if (LayerMask.NameToLayer("Enemy") != -1 && collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            float playerBottom = transform.position.y - 0.5f;

            if (playerBottom > contactPoint.y)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
            else
            {
                Die();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || myCollider == null) return;
        
        // Draw the ground check sphere
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPosition, groundCheckRadius);
        
        // Draw the collider bounds
        Gizmos.color = Color.yellow;
        Bounds bounds = myCollider.bounds;
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        
        // Draw lines at the corners of the collider
        Gizmos.DrawLine(new Vector3(center.x - size.x/2, center.y - size.y/2, center.z),
                       new Vector3(center.x + size.x/2, center.y - size.y/2, center.z));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered with {other.gameObject.name}, tag: {other.tag}");
        
        if (other.CompareTag("Coin"))
        {
            CollectCoin();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    private void CollectCoin()
    {
        coins++;
        Debug.Log($"=== Coin Collection ===");
        Debug.Log($"Collected coin! Total coins: {coins}/10");

        if (coins >= 10)
        {
            Debug.Log("=== Golden Platform Activation ===");
            Debug.Log("Reached 10 coins! Searching for golden platform...");
            
            // First, verify the tag exists
            if (LayerMask.NameToLayer("GoldenPlatform") == -1)
            {
                Debug.LogError("GoldenPlatform tag is not defined in Tags & Layers!");
                return;
            }

            // Try to find ALL objects with the tag
            GameObject[] allPlatforms = GameObject.FindGameObjectsWithTag("GoldenPlatform");
            Debug.Log($"Found {allPlatforms.Length} platforms with GoldenPlatform tag");

            if (allPlatforms.Length > 0)
            {
                foreach (GameObject platform in allPlatforms)
                {
                    Debug.Log($"Platform found: {platform.name}");
                    Debug.Log($"Platform active state before: {platform.activeSelf}");
                    platform.SetActive(true);
                    Debug.Log($"Platform active state after: {platform.activeSelf}");
                }
            }
            else
            {
                Debug.LogError("No golden platforms found! Checking all objects in scene:");
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                Debug.Log("=== All Scene Objects ===");
                foreach (GameObject obj in allObjects)
                {
                    Debug.Log($"- {obj.name} (tag: {obj.tag})");
                }
            }
        }
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coins}";
        }
        else
        {
            Debug.LogWarning("CoinText reference is missing!");
        }
    }

    public void Die()
    {
        isGameOver = true;
        rb.velocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        
        // Show game over text
        GameObject gameOverText = GameObject.FindGameObjectWithTag("GameOverText");
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }
    }

    public void Win()
    {
        hasWon = true;
        rb.velocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetTrigger("Win");
        }
        
        // Show win text
        GameObject winText = GameObject.FindGameObjectWithTag("WinText");
        if (winText != null)
        {
            winText.SetActive(true);
        }
    }
} 