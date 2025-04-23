using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlatformerPlayer : MonoBehaviour
{
    [SerializeField]
    private float speed = 4.5f;
    private Rigidbody2D body;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        Vector2 movement = new Vector2(deltaX, body.velocity.y);
        body.velocity = movement;
    }
}