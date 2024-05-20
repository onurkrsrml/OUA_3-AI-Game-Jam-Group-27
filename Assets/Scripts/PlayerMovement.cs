using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool gatherInput;

    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float turnControl;
    public bool facingRight { get; private set; } = true;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpControl;

    [Header("Gravity")]
    [SerializeField] private float gravity;
    [SerializeField] private float fallGravity;
    [SerializeField] private float maximumFallVelocity;

    [Header("Jump Checks")]
    [SerializeField] private float bufferTime;
    [SerializeField] private float hangTime;
    private float bufferTimeCounter;
    private float hangTimeCounter;

    [Header("Bouncing")]
    [SerializeField] private float bounceJumpForce;
    private bool isInBounceJump;

    [Header("Collision Checks")]
    [SerializeField] private float groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask bouncePadLayer;
    public bool isOnGround { get; private set; } = true;
    private bool wasOnGround = true;
    private bool isOnBouncePad;

    [Header("Visuals")]
    [SerializeField] private GameObject graphics;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem dustParticles;

    //Input
    private float moveInput;
    private bool jumpInput;
    private bool releaseJumpInput;
    private bool holdJumpInput;
    private bool previousJumpInput;

    //Components
    private Rigidbody2D rb;
    private BoxCollider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        GatherInput();
        SetDirection();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        Move();
        CheckJumpAllowed();
        Jump();
        ModifyJump();
    }

    private void GatherInput()
    {
        if (!gatherInput)
        {
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpInput = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            releaseJumpInput = true;
        }

        holdJumpInput = Input.GetButton("Jump");
    }

    public void SetInput(float _moveInput, float _jumpInput)
    {
        moveInput = _moveInput;

        jumpInput = !previousJumpInput && _jumpInput == 1;
        releaseJumpInput = previousJumpInput && _jumpInput == 0;
        holdJumpInput = previousJumpInput && _jumpInput == 1;

        previousJumpInput = _jumpInput == 1;
    }

    private void SetDirection()
    {
        facingRight = graphics.transform.rotation.eulerAngles.y == 0;

        if (moveInput < 0 && facingRight || moveInput > 0 && !facingRight)
        {
            graphics.transform.rotation = Quaternion.Euler(0, facingRight ? 180 : 0, 0);
        }
    }

    private void CheckCollisions()
    {
        wasOnGround = isOnGround;
        isOnGround = Physics2D.OverlapBox(rb.position + new Vector2(0, -col.size.y / 2 - col.edgeRadius), new Vector2(groundCheckSize, 0), 0, groundLayer);
        //animator.SetBool("isOnGround", onGround);

        isOnBouncePad = Physics2D.OverlapBox(rb.position + new Vector2(0, -col.size.y / 2 - col.edgeRadius), new Vector2(groundCheckSize, 0), 0, bouncePadLayer);

        if (isOnBouncePad && !isInBounceJump)
        {
            StartCoroutine(BounceJump());
        }

        if (!wasOnGround && isOnGround)
        {
            dustParticles.Play();
        }
    }

    private void Move()
    {
        if (moveInput != 0)
        {
            if (Mathf.Sign(moveInput) != Mathf.Sign(rb.velocity.x))
            {
                rb.velocity = new Vector2(rb.velocity.x * (1 - turnControl), rb.velocity.y);
            }

            if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.AddForce(moveInput * acceleration * Vector2.right);
            }
        }
        else
        {
            if (Mathf.Abs(rb.velocity.x) > 1)
            {
                rb.AddForce(Mathf.Sign(rb.velocity.x) * acceleration * Vector2.left);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        //animator.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
    }

    private void CheckJumpAllowed()
    {
        if (jumpInput)
        {
            jumpInput = false;
            bufferTimeCounter = bufferTime;
        }
        else
        {
            bufferTimeCounter -= Time.fixedDeltaTime;
        }

        if (isOnGround)
        {
            hangTimeCounter = hangTime;
        }
        else
        {
            hangTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void Jump()
    {
        if (bufferTimeCounter > 0 && hangTimeCounter > 0)
        {
            bufferTimeCounter = 0;
            hangTimeCounter = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            dustParticles.Play();
        }
    }

    private void ModifyJump()
    {
        if (isInBounceJump)
        {
            rb.gravityScale = gravity;
            return;
        }

        if (releaseJumpInput)
        {
            releaseJumpInput = false;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * (1 - jumpControl));
            }
        }

        if (rb.velocity.y < 0 || !holdJumpInput)
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, maximumFallVelocity));
        //animator.SetFloat("velocityY", rb.velocity.y);
    }

    private IEnumerator BounceJump()
    {
        isInBounceJump = true;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(bounceJumpForce * Vector2.up, ForceMode2D.Impulse);
        yield return new WaitUntil(() => rb.velocity.y < 0);
        isInBounceJump = false;
    }
}