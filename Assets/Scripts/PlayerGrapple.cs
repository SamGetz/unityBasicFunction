using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerGrapple : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleSpeed = 25f;  // Speed at which the player moves to the grapple point.
    public float maxGrappleDistance = 20f;  // Maximum distance for grappling.
    public float grappleShootSpeed = 60f;  // Speed at which the grapple line shoots to the target.
    public LayerMask grappleMask;  // Layer mask to determine what the grapple can attach to.

    [Header("Grapple End Offset")]
    public float plungerStickLength = 0.5f;  // Offset length for where the grapple attaches.
    public Vector3 lineStartOffset = new Vector3(0, 2.2f, 0);  // Offset for the start of the grapple line.

    [Header("UI Elements")]
    public LineRenderer grappleLine;  // Line renderer for the grapple line.
    public GameObject grappleEndSprite;  // Sprite representing the end of the grapple.

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 grapplePoint;  // Point where the grapple attaches.
    private Vector2 grappleLineEndPoint;  // Point where the grapple line ends.
    private bool isGrappling;
    private bool isShootingGrapple;
    private Coroutine grappleCoroutine;
    private SpriteRenderer grappleEndSpriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grappleEndSpriteRenderer = grappleEndSprite.GetComponent<SpriteRenderer>();

        grappleLine.enabled = false;
        grappleEndSprite.SetActive(false);
    }

    void Update()
    {
        HandleGrappleInput();
    }

    private void HandleGrappleInput()
    {
        // Check for input to start or stop grappling.
        if (Input.GetButtonDown("Grapple") && !isGrappling && !isShootingGrapple)
        {
            anim.SetTrigger("GrappleOn");
            AttemptGrapple();
        }
        else if (Input.GetButtonUp("Grapple") || (isGrappling && Input.GetButtonDown("Jump")))
        {
            StopGrapple();
        }

        if (isGrappling)
        {
            PerformGrapple();
        }
    }

    private void AttemptGrapple()
    {
        // Try to initiate a grapple based on mouse position.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxGrappleDistance, grappleMask);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            grapplePoint = hit.point;
            grappleEndSprite.transform.position = grapplePoint;
            grappleLineEndPoint = grapplePoint - (hit.normal * plungerStickLength);  // Calculate the offset based on the hit normal.

            float distanceToTarget = Vector2.Distance(transform.position, grapplePoint);
            float timeToReachTarget = distanceToTarget / grappleShootSpeed;

            if (grappleCoroutine != null)
            {
                StopCoroutine(grappleCoroutine);
            }

            grappleCoroutine = StartCoroutine(ShootGrapple(grapplePoint, timeToReachTarget));
        }
    }

    private IEnumerator ShootGrapple(Vector2 target, float timeToReachTarget)
    {
        // Animate the grapple shooting towards the target.
        isShootingGrapple = true;
        grappleLine.enabled = true;
        grappleEndSprite.SetActive(true);
        float t = 0;

        while (t < timeToReachTarget)
        {
            if (!isShootingGrapple)
            {
                StopGrapple();
                yield break;
            }

            t += Time.deltaTime;
            Vector2 currentPos = Vector2.Lerp(transform.position, target, t / timeToReachTarget);

            grappleLine.SetPosition(0, transform.position + lineStartOffset);
            grappleLine.SetPosition(1, currentPos);

            grappleEndSprite.transform.position = currentPos;

            yield return null;
        }

        grappleLine.SetPosition(1, grappleLineEndPoint);
        isShootingGrapple = false;
        StartGrapple();
    }

    private void StartGrapple()
    {
        // Initiate the grappling process.
        isGrappling = true;
        grappleEndSprite.SetActive(true);
        anim.SetTrigger("GrappleOn");
        rb.velocity = Vector2.zero;

        grappleLine.SetPosition(0, transform.position + lineStartOffset);
        grappleLine.SetPosition(1, grappleLineEndPoint);
    }

    private void StopGrapple()
    {
        // Stop the grapple and reset everything.
        isGrappling = false;
        isShootingGrapple = false;
        anim.ResetTrigger("GrappleOn");
        grappleLine.enabled = false;
        grappleEndSprite.SetActive(false);

        if (grappleCoroutine != null)
        {
            StopCoroutine(grappleCoroutine);
            grappleCoroutine = null;
        }
        rb.velocity = Vector2.zero;
    }

    private void PerformGrapple()
    {
        // Move the player towards the grapple point.
        grappleLine.SetPosition(0, transform.position + lineStartOffset);
        grappleLine.SetPosition(1, grappleLineEndPoint);

        Vector2 grappleDirection = (grapplePoint - (Vector2)transform.position).normalized;
        rb.velocity = grappleDirection * grappleSpeed;
    }
}