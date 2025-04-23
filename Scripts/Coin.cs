using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    private BoxCollider2D boxCollider;

    private void Start()
    {
        Debug.Log($"=== Coin {gameObject.name} Initialization ===");
        
        // Get and setup the box collider
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
            Debug.Log($"BoxCollider2D found and set as trigger. Size: {boxCollider.size}, Offset: {boxCollider.offset}, Enabled: {boxCollider.enabled}");
        }
        else
        {
            Debug.LogError($"BoxCollider2D is missing on {gameObject.name}!");
            return;
        }

        // Check if we're on the correct layer
        Debug.Log($"Coin is on layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)})");
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"OnTriggerEnter2D detected! Colliding with: {other.gameObject.name}");
        Debug.Log($"Other object's layer: {other.gameObject.layer} ({LayerMask.LayerToName(other.gameObject.layer)})");
        Debug.Log($"Other object's position: {other.transform.position}, Coin position: {transform.position}");
        
        // Play effects
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Destroy the coin
        Destroy(gameObject);
    }

    // Add these methods to check if they get called instead
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"OnCollisionEnter2D detected with: {collision.gameObject.name}");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"OnTriggerStay2D with: {other.gameObject.name}");
    }
} 