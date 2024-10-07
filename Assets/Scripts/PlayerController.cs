using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 5f;
    public float airWalkSpeed = 2.5f;
    public float runSpeed = 10f;
    public float jumpImpulse = 10f;

    TouchingDirections touchingDirections;
    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving && !touchingDirections.IsOnWall)
            {
                if (touchingDirections.IsGrounded)
                {
                    if (IsRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    return airWalkSpeed;
                }

            }
            return 0f;

        }
    }
    public bool IsAlive
    {
        get
        {
            return animator.GetBool("isAlive");
        }
    }
    Vector2 moveInput;
    [SerializeField]
    private bool _isMoving = false;
    [SerializeField]
    public bool IsMoving
    {
        get
        {

            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }
    private bool _isRunning = false;
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool("isRunning", value);
        }
    }
    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
            {
                //Flip the local scale to make the player face the opposite direction
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    private void FixedUpdate()
    {

        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        // rb.velocity = new Vector2(moveInput.x * walkSpeed * Time.fixedDeltaTime, rb.velocity.y);
        animator.SetFloat("yVelocity", rb.velocity.y);
        if (!IsAlive)
        {

            //pause game after 1s
            StartCoroutine(PauseGameAfterDelay(3f));  // Start the coroutine to pause after 1 second


        }

    }

    private IEnumerator PauseGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        Time.timeScale = 0;  // Pause the game
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }

    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            //Face the right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        //TODO: Check if alive as well
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            animator.SetBool("isComboAttack", false);
            animator.SetTrigger("attack");
        }
    }
    public void OnHit(int damage, Vector2 knockback)
    {
        //if want ez game -> remove this
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
    public void OnComboAttack(InputAction.CallbackContext context)
    {
        Debug.Log(">>> GAME OBJECT ON COMBOATTACK: " + gameObject.name);
        if (context.started)
        {
            animator.SetTrigger("attack");
            animator.SetBool("isComboAttack", true);
        }
        if (context.canceled)
        {
            animator.SetBool("isComboAttack", false);
        }
    }
}
