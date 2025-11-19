using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public characterController controller;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;
    public float skinWidth = 0.02f;
    public bool stickToGround = true;

    [Header("Physics Settings")]
    public float gravity = -25f;

    [Header("State")]
    public bool isGrounded;
    public Vector2 velocity;    
    public Vector2 inputDirection;

    private Vector2 groundNormal = Vector2.up;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // tu controla a gravidade
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        DetectGround();
        ApplyGravity();
        MoveCharacter();
    }

    // ─────────────────────────────────────────────
    // GROUND CHECK
    // ─────────────────────────────────────────────
    void DetectGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            Vector2.down,
            groundCheckDistance + skinWidth,
            groundMask
        );

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
            }
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector2.up;
        }
    }

    // ─────────────────────────────────────────────
    // GRAVITY
    // ─────────────────────────────────────────────
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0;
        }
    }

    // ─────────────────────────────────────────────
    // MOVEMENT (posicionamento manual + RB)
    // ─────────────────────────────────────────────
    void MoveCharacter()
    {
        Vector2 delta = velocity * Time.fixedDeltaTime;
        rb.position += delta;
    }

    // ─────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────

    public void SetDirection(Vector2 dir) => inputDirection = dir;

    public void SetVelocity(Vector2 vel) => velocity = vel;

    public void AddForce(Vector2 force) => velocity += force;

    public void StopHorizontalForce() => velocity.x = 0;

    public void Jump(float force)
    {
        if (isGrounded)
            velocity.y = force;
    }

    public void DampMovement(float damping)
    {
        velocity.x = Mathf.Lerp(velocity.x, 0, damping * Time.deltaTime);
    }

    // Helpers
    public float HorizontalSpeed => velocity.x;

    public int FacingDirection =>
        velocity.x > 0.1f ? 1 :
        velocity.x < -0.1f ? -1 : 0;

    public bool IsFalling => velocity.y < -0.1f;
    public bool IsRising => velocity.y > 0.1f;

    // ─────────────────────────────────────────────
    // GIZMOS
    // ─────────────────────────────────────────────
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = Application.isPlaying ? (Vector3)rb.position : transform.position;
        Vector3 end = origin + Vector3.down * (groundCheckDistance + skinWidth);
        Gizmos.DrawLine(origin, end);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, 0.04f);

        Gizmos.color = Color.green;
        if (Application.isPlaying)
            Gizmos.DrawLine(origin, origin + (Vector3)velocity * 0.1f);
    }
}
