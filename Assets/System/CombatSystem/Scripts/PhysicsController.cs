using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public characterController controller;
    [Header("Ground")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;
    public float skinWidth = 0.02f;
    public bool stickToGround = true;

    [Header("Physics")]
    public float gravity = -30f;
    public float maxFallSpeed = -40f;

    [Header("State")]
    public bool isGrounded;
    public Vector2 velocity; // controlada pelo movimento
    Vector2 groundNormal = Vector2.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        DetectGround();
        ApplyGravity();
        ApplyVelocity();
    }

    //──────────────────────────────────────────────
    // GROUND CHECK
    //──────────────────────────────────────────────
    void DetectGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            Vector2.down,
            groundCheckDistance + skinWidth,
            groundMask);

        if (hit.collider != null)
        {
            isGrounded = true;
            groundNormal = hit.normal;

            if (stickToGround)
            {
                rb.position = new Vector2(
                    rb.position.x,
                    hit.point.y + skinWidth
                );
                if (velocity.y < 0) velocity.y = 0;
            }
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector2.up;
        }
    }

    //──────────────────────────────────────────────
    // GRAVITY CONTROLADA
    //──────────────────────────────────────────────
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, maxFallSpeed);
        }
    }

    //──────────────────────────────────────────────
    // MOVIMENTO REAL (via Rigidbody)
    //──────────────────────────────────────────────
    void ApplyVelocity()
    {
        rb.linearVelocity = velocity;
    }

    //──────────────────────────────────────────────
    //  PUBLIC API (ESSA É A PARTE IMPORTANTE)
    //──────────────────────────────────────────────

    public void SetHorizontal(float x)
    {
        velocity.x = x;
    }

    public void SetVertical(float y)
    {
        velocity.y = y;
    }

    public void AddForce(Vector2 force)
    {
        velocity += force;
    }

    public void ApplyKnockback(Vector2 force)
    {
        velocity = force;
    }

    public void StopHorizontal()
    {
        velocity.x = 0;
    }

    public void StopVertical()
    {
        velocity.y = 0;
    }

    // Bounce vertical simples
    public void Bounce(float force)
    {
        velocity.y = force;
    }
}
