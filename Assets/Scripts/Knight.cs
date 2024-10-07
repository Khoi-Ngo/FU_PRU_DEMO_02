using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public DetectionZone attackZone;
    Animator animator;
    public float walkSpeed = 3f;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;

    public float maxTravelDistance = 5f; // Max distance to travel before flipping
    private Vector2 startPosition; // Position when the knight starts walking in a direction

    public enum WalkableDirection { Right, Left };
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Flip direction
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }

                // Update the start position when changing direction
                startPosition = transform.position;
            }
            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();

        // Set the initial position to the starting point of the knight
        startPosition = transform.position;
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool("isAlive");
        }
    }

    void Update()
    {
        // Check if there is a detected target
        if (attackZone.detectedColliders.Count > 0)
        {
            // Get the Damageable component of the target
            Damageable targetDamageable = attackZone.detectedColliders[0].GetComponent<Damageable>();

            // Check if the target is not null and if it's alive
            if (targetDamageable != null && targetDamageable.IsAlive)
            {
                HasTarget = true;

                // Check the relative position of the detected object
                Collider2D target = attackZone.detectedColliders[0];
                float targetPositionX = target.transform.position.x;

                // Flip direction based on target's position
                if (targetPositionX > transform.position.x && WalkDirection == WalkableDirection.Left)
                {
                    WalkDirection = WalkableDirection.Right;
                }
                else if (targetPositionX < transform.position.x && WalkDirection == WalkableDirection.Right)
                {
                    WalkDirection = WalkableDirection.Left;
                }
            }
            else
            {
                // If the target is dead, stop attacking
                HasTarget = false;
            }
        }
        else
        {
            HasTarget = false;
        }

        // Stop movement if this bot is dead
        if (!IsAlive)
        {
            walkSpeed = 0;
        }

        // Check if the knight has traveled more than the max distance
        float distanceTraveled = Mathf.Abs(transform.position.x - startPosition.x);
        if (distanceTraveled >= maxTravelDistance)
        {
            FlipDirection();
        }
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Unknown walk direction!");
        }
    }
    public void OnHit(int damage, Vector2 knockback)
    {
        float newX = rb.velocity.x >= 0 ? rb.velocity.x + 1 : rb.velocity.x - 1;
        rb.velocity = new Vector2(newX, rb.velocity.y + knockback.y);

    }
}
