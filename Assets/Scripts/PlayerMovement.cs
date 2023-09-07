using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveForce = 12f;
    [SerializeField]

    private bool isJumping = false;
    private bool isGrounded = false;

    private float movementX;
    private float movementY;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private string RUN_ANIMATION = "Run";

    private string GROUND_TAG = "Ground";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);

        PlayerMoveKeyboard();
        RunAnimation();
    }


    void PlayerMoveKeyboard()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        movementY = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }
    void RunAnimation() {

        if (movementX > 0 && isGrounded)
        {
            sr.flipX = false;
            anim.SetBool(RUN_ANIMATION, true);
        }
        else if (movementX < 0 && isGrounded)
        {
            sr.flipX = true;
            anim.SetBool(RUN_ANIMATION, true);
        }
        else
        {
            anim.SetBool(RUN_ANIMATION, false);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            isJumping = false;
            isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    // Leaves the ground
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            isGrounded = false;
        }
    }
}
