using UnityEngine;

public class GoldenPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float riseSpeed = 3f;
    [SerializeField] private float maxRiseHeight = 20f;
    
    private bool isRising;
    private Vector3 startPosition;
    private bool hasReachedTop;

    private void Start()
    {
        startPosition = transform.position;
        gameObject.SetActive(false); // Start inactive until player collects enough coins
    }

    private void Update()
    {
        if (isRising && !hasReachedTop)
        {
            // Move platform upward
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);

            // Check if we've reached the maximum height
            if (transform.position.y >= startPosition.y + maxRiseHeight)
            {
                hasReachedTop = true;
                isRising = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isRising && !hasReachedTop)
        {
            // Start rising when player lands on the platform
            isRising = true;

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
} 