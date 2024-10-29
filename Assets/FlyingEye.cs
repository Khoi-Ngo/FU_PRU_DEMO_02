using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    public DetectionZone biteDetectionZone;
    public List<Transform> waypoints;
    Damageable damageable;

    Transform nextWaypoint;
    int waypointNum = 0;

    Animator animator;
    Rigidbody2D rb;

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
    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }


    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            // dead FlyingEye falls to the ground
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    private void Flight()
    {
        // fly to the next waypoint 
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;
        // Check if we have reached the waypont already
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        //See if we need to switch waypoint
        if (distance <= waypointReachedDistance)
        {
            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                // Loop back to the original waypoint
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;

        if (transform.localScale.x > 0)

        {
            // facing the right
            if (rb.velocity.x < 0)
            {
                // flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);

            }
        }

        else
        {
            // facing the left
            if (rb.velocity.x > 0)
            {
                // flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);

            }
        }
    }
}
