using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    public Transform attackPoint;

    public float attackSpeed = 0.2f;
    public int attackPower = 50;
    public float attackRange = 0.5f;

    public Vector2 knockbackForce = new Vector2(300, 0);
    private bool canAttack = true;
    private float movementX;
    public LayerMask enemyLayers;
    private Vector2 directionalKnockback;

    private string MINE_ANIMATION = "Mine";

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {

        movementX = Input.GetAxisRaw("Horizontal");

        // Update attackPoint position based on player facing direction
        if (movementX != 0)
        {
            
            Vector3 attackPointPosition = attackPoint.localPosition;
            attackPointPosition.x = Mathf.Abs(attackPointPosition.x) * Mathf.Sign(movementX);
            directionalKnockback.x = Mathf.Abs(knockbackForce.x) * Mathf.Sign(movementX);
            attackPoint.localPosition = attackPointPosition;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        else
        {
            anim.SetBool(MINE_ANIMATION, false);
        }
    }
    void Attack()
    {
        // Play an attack animation
        anim.SetTrigger(MINE_ANIMATION);


        // Detect enemies in the range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackPower, directionalKnockback);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) 
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}


