using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    private Vector2 movement;

    private Rigidbody2D rb;
    private Animator animator;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    private const string lasthorizontal = "LastHorizontal";
    private const string lastvertical = "LastVertical";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat(horizontal, movement.x);
        animator.SetFloat(vertical, movement.y);

        rb.linearVelocity = movement * moveSpeed;

        if (movement != Vector2.zero)
        {
            animator.SetFloat(lasthorizontal, movement.x);
            animator.SetFloat(lastvertical, movement.y);
        }
    }
}
