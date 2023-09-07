using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector3 joltAmount = new Vector3(0.1f, 0, 0);  // The distance to knockback the enemy

    public int maxHealth = 100;
    int currentHealth;

    private Animator anim;
    private SpriteRenderer spriteRend;
    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    public Collider2D playerCollider;  // Reference to the player's collider


    public float flashDuration = 0.1f;
    private Color originalColor;

    public float joltDuration = 0.05f;  // Duration of the jolt in seconds

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        spriteRend = GetComponent<SpriteRenderer>();
        originalColor = spriteRend.color;
        enemyCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }


    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        currentHealth -= damage;
        ApplyKnockback(knockbackForce);
        StartCoroutine(FlashRed());
        StartCoroutine(JoltEnemy());
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {

        if (enemyCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
        }
        // Die animation
        // Disable Enemy.
        Debug.Log("Enemy died!");
        anim.SetBool("IsDead", true);
        this.enabled = false;
        
    }

    private void ApplyKnockback(Vector2 knockbackForce)
    {
        rb.drag = 0;  // Reset drag
        rb.AddForce(knockbackForce);
        StartCoroutine(StopKnockback());
    }

    private IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(0.1f);  // Wait for a short duration
        rb.drag = 4;  // Increase drag for sudden stop
    }
    private IEnumerator FlashRed()
    {
        // Change the sprite color to red
        spriteRend.color = Color.red;
        // Wait for the flash duration
        yield return new WaitForSeconds(flashDuration);
        spriteRend.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        spriteRend.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        // Revert the sprite color back to its original color
        spriteRend.color = originalColor;
    }

    private IEnumerator JoltEnemy()
    {
        // Store the original position
        Vector3 originalPosition = transform.position;

        // Move the enemy to the jolt position
        transform.position += joltAmount;

        // Wait for the jolt duration
        yield return new WaitForSeconds(joltDuration);

        // Revert the enemy position back to its original position
        transform.position = originalPosition;
    }
}
