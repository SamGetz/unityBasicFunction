using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles the jumping mechanics for the player character in a 2D game.
public class PlayerJumping : MonoBehaviour
{
    // Tag used to identify ground objects.
    private string GROUND_TAG = "Ground";

    // Should the fall multiplier be applied? If false, natural gravity will be used.
    private bool fallMutliplierOn = true;

    // Variables controlling jump and fall behavior.
    public float fallMultiplier = 8f;  // Multiplier for faster falling.
    public float lowJumpMultiplier = 10f;  // Multiplier for low jump.
    public float jumpForce = 15.0f;  // Force applied when jumping.
    public float jumpDampenTimer = 0.3f;  // Time until jump peak dampening starts.
    public float jumpPeakDampen = 0.2f;  // Dampening applied to jump peak.
    public float maxFallSpeed = -18f;  // Maximum speed the player can fall.

    // Internal state variables.
    private bool isJumping = false;
    private bool isGrounded = false;
    private float movementX;
    private float movementY;

    // Animation states.
    private string JUMP_UP_ANIMATION = "JumpUp";
    private string JUMP_PEAK_ANIMATION = "JumpPeak";
    private string FALLING_ANIMATION = "Falling";

    // Components.
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    // Initialization.
    private void Awake()
    {
        // Get the Rigidbody2D and Animator components.
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame.
    void Update()
    {
        // Apply the fall and jump multipliers if enabled.
        if (fallMutliplierOn)
        {
            ApplyFallMultiplier();
        }
        movementX = Input.GetAxisRaw("Horizontal");
        movementY = Input.GetAxisRaw("Vertical");
        // Handle player input for jump.
        if (Input.GetButtonDown("Jump") && !isJumping && isGrounded)
        {
            StartCoroutine(JumpPeakDampening());
            PlayerJump(Vector2.up);
        }

        // Handle jump animations.
        JumpAnimation();
    }

    // FixedUpdate is called at fixed intervals.
    private void FixedUpdate()
    {
        // Clamp the falling speed.
        ClampFallSpeed();
    }

    // Applies the fall and low jump multipliers to the player's jump.
    void ApplyFallMultiplier()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Execute the jump with the given direction.
    void PlayerJump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        isJumping = true;
        isGrounded = false;
    }

    void JumpAnimation()
    {
        if (rb.velocity.y > 1.5 && !isGrounded)
        {
            anim.SetTrigger(JUMP_UP_ANIMATION);
        }
        else if (rb.velocity.y < 1.5 && rb.velocity.y > 0 && !isGrounded)
        {
            anim.SetTrigger(JUMP_PEAK_ANIMATION);
        }
        else if (rb.velocity.y < 0.5 && !isGrounded)
        {
            anim.SetTrigger(FALLING_ANIMATION);
        }
        if (isGrounded)
        {
            anim.SetBool(FALLING_ANIMATION, false);
            anim.SetBool(JUMP_PEAK_ANIMATION, false);
            anim.SetBool(JUMP_UP_ANIMATION, false);
        }
        // Handle left and right sprite display.
        if (isJumping)
        {
            if (movementX > 0 && isJumping)
            {
                sr.flipX = false;
            }
            else if (movementX < 0 && isJumping)
            {
                sr.flipX = true;
            }
        }
    }
    
    // Clamps the fall speed to the maximum value.
    void ClampFallSpeed()
    {
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    // Coroutine that dampens the jump peak.
    IEnumerator JumpPeakDampening()
    {
        yield return new WaitForSeconds(jumpDampenTimer);
        rb.velocity *= jumpPeakDampen;
    }

    // Handles ground collision.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            isJumping = false;
            isGrounded = true;
        }
    }

    // Handles ground exit.
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            isGrounded = false;
            isJumping = true;
        }
    }
}