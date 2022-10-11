using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float slideSpeed;
    public float groundDrag;

    [Header("Jumping")]
    [SerializeField] float jumpForce;

    [Header("Scaling")]
    float originScale;
    public float crouchScale;
    
    [Header("Sliding")]
    public float slideDuration;
    float slidingDuration;

    [Header("Dashing")]
    public float dashDuration;
    public float dashCoolDown;
    public float dashPower;

    [Header("Ground Pound")]
    public float groundPoundForce;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.Mouse0;
    public KeyCode groundPoundKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    bool grounded;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    bool canJump;
    bool canCrouch;
    bool canSlide;
    bool canDash;

    MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        dashing,
        groundpounding,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        originScale = transform.localScale.y;
        slidingDuration = slideDuration;

        canJump = true;
        canCrouch = true;
        canSlide = false;
        canDash = true;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        SpeedControl();
        StateHandler();
        Crouch();
        Slide();
        MyInput();

        rb.drag = groundDrag;
    }

    void FixedUpdate()
    {
        MovePlayer();
        Crouch();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump == true && grounded)
        {
            canJump = false;
            Jump();
            canJump = true;
        }

        if (Input.GetKeyDown(dashKey) && canDash)
        {
            StartCoroutine(Dash());
        }
        StopCoroutine(Dash());

        if (Input.GetKey(groundPoundKey))
        {
            GroundPound();
        }
    }

    void StateHandler()
    {
        if (!grounded) 
        { 
            state = MovementState.air;
        }

        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        if (Input.GetKey(crouchKey) && canCrouch && !canSlide)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (Input.GetKey(crouchKey) && canSlide && !canCrouch && grounded || canDash == false)
        {
            state = MovementState.sliding;
            moveSpeed = slideSpeed;
        }

        if (Input.GetKeyDown(groundPoundKey))
        {
            state = MovementState.groundpounding;
        }
    }

    void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, 0f, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void scaleDown()
    {
        if (Input.GetKey(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchScale, transform.localScale.z);
        }
        rb.AddForce(Vector3.down * 7f);
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, originScale, transform.localScale.z);
        }
    }

    void Crouch()
    {
        scaleDown();
        if (moveSpeed <= walkSpeed)
        {
            canCrouch = true;
            canSlide = false;
        }
    }

    void Slide()
    {
        scaleDown();
        if (moveSpeed > walkSpeed)
        {
            canCrouch = false;
            canSlide = true;
        }

        if (state == MovementState.sliding)
        {
            slidingDuration -= Time.deltaTime;
            ResetSlide();
        }
    }

    void ResetSlide()
    {
        if (slidingDuration <= 0)
        {
            canSlide = false;
            if (state == MovementState.crouching)
            {
                moveSpeed = crouchSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
            slidingDuration = slideDuration;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        state = MovementState.dashing;
        rb.useGravity = false;
        rb.AddForce(orientation.forward * dashPower, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        rb.useGravity = true;
        if (grounded)
        {
            state = MovementState.walking;
        } 
        else
        {
            state = MovementState.air;
        }
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    void GroundPound()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.down * groundPoundForce, ForceMode.Force);
    }
}
