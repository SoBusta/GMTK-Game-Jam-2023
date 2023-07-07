using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Initializing Player Rigidbody
    private Rigidbody2D Player;

    // Initializing Player Collider
    private BoxCollider2D playerCollider;

    // Initializing Player Sprite Renderer
    private SpriteRenderer sp;

    // Obstacles
    [SerializeField] public LayerMask nonTraversableObstacle;
    [SerializeField] public LayerMask TraversableObstacle;

    //Box Collidiers
    bool onGround;
    bool onCeilling;
    bool onLeftWall;
    bool onRightWall;

    // Keyboard Inputs
    private bool rightArrowButton;
    private bool leftArrowButton;
    private bool downArrowButton;

    private bool holdingJumpButton;
    private bool holdingDashButton;

    private bool pressedJumpButton;
    private bool pressedDashButton;

    public Vector2 mouseDirection;
    public float mouseDirectionX; // Detects in which direction the player is going discretly in the x-axis (-1, 0, 1)
    public float mouseDirectionY; // Detects in which direction the player is going discretly in the y-axis (-1, 0, 1)

    // Gravity
    [SerializeField] private float gravityAcc = 25f;

    // Jump Variables
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private float coyoteTimeThreshold = 0.2f;
    private float coyoteTimeCounter;
    private bool canCoyote;

    [SerializeField] private float jumpBufferTimeThreshold = 0.2f;
    private float jumpBufferTimeCounter;
    private bool canJumpBuffer;

    // Left-Right
    [SerializeField] private float accelerationFactor = 3f;
    [SerializeField] private float PlayerTopSpeed = 15f;
    float DirectionalValue;


    // Start is called before the first frame update
    void Start()
    {
        // Initializing Rigidbody
        Player = GetComponent<Rigidbody2D>();

        // Initializing Player's Collider
        playerCollider = GetComponent<BoxCollider2D>();

        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // User Inputs
        rightArrowButton = Input.GetKey(KeyCode.D);
        leftArrowButton = Input.GetKey(KeyCode.A);
        downArrowButton = Input.GetKey(KeyCode.S);

        pressedJumpButton = Input.GetKeyDown(KeyCode.Space);
        pressedDashButton = Input.GetKeyDown(KeyCode.LeftShift);

        holdingJumpButton = Input.GetKey(KeyCode.Space);
        holdingDashButton = Input.GetKey(KeyCode.LeftShift);

        //mouseDirection = (playerInput.actions["mousePosition"].ReadValue<Vector2>() - playerPosition).normalized;

        // Detect Trivial Collisions
        onGround = CheckCollision(Player.position, Vector2.down, 0.2f, nonTraversableObstacle);
        onCeilling = CheckCollision(Player.position, Vector2.up, 0.1f, nonTraversableObstacle);
        onLeftWall = CheckCollision(Player.position, Vector2.left, 0.1f, nonTraversableObstacle);
        onRightWall = CheckCollision(Player.position, Vector2.right, 0.1f, nonTraversableObstacle);

        // Coyote Time
        coyoteTimeCounter = ConditionalTimerDuration(coyoteTimeCounter, coyoteTimeThreshold, onGround);
        coyoteTimeCounter = Timer(coyoteTimeCounter);
        canCoyote = CheckCondition(coyoteTimeCounter, canCoyote);

        // Jump Buffer Time
        jumpBufferTimeCounter = ConditionalTimerDuration(jumpBufferTimeCounter, jumpBufferTimeThreshold, pressedJumpButton);
        jumpBufferTimeCounter = Timer(jumpBufferTimeCounter);
        canJumpBuffer = CheckCondition(jumpBufferTimeCounter, canJumpBuffer);
    }

    // Fixed Update is called every interval
    void FixedUpdate()
    {
        // Get Direction Player wants to go
        if (leftArrowButton && !rightArrowButton)
        {
            DirectionalValue = -1;

            //Flips the sprite
            sp.flipX = true;
        }
        else if (!leftArrowButton && rightArrowButton)
        {
            DirectionalValue = 1;

            //Flips the sprite
            sp.flipX = false;
        }
        else
        {
            DirectionalValue = 0;
        }

        // Gravity
        GravityPhysics();

        // Jump
        JumpPhysics();

        // Left-Right Movement
        DirectionalPhysics();

    }

    // Gravity Physics
    void GravityPhysics()
    {
        Player.AddForce(Vector2.down * gravityAcc);
    }

    // Jump Physics
    void JumpPhysics()
    {
        if (canCoyote && canJumpBuffer)
        {
            // If the player is going down, we set velocity in "y" to 0 so that the jump feels normal
            if (Player.velocity.y < 0)
            {
                Player.velocity = new Vector2(Player.velocity.x, 0);
            }

            // Setting Jump Force in "y" direction as well as a little boost in "x" if allowed to give it more dynamisme
            Player.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

            // Setting the "Coyote" and "JumpBuffer" timers back to 0
            jumpBufferTimeCounter = ConditionalTimerDuration(jumpBufferTimeCounter, 0, true);
            coyoteTimeCounter = ConditionalTimerDuration(coyoteTimeCounter, 0, true);
        }
    }

    // Left And Right Physics
    void DirectionalPhysics()
    {
        if (DirectionalValue != 0)
        {
            // Calculate the acceleration based on the difference between current velocity and top velocity
            float acceleration = Mathf.Abs(DirectionalValue * PlayerTopSpeed - Player.velocity.x) * accelerationFactor;

            // Apply the acceleration to the rigidbody's velocity
            Player.AddForce(new Vector2(DirectionalValue, 0) * acceleration, ForceMode2D.Force);

            // Limit the velocity to the top speed
            Player.velocity = new Vector2(Mathf.Clamp(Player.velocity.x, -PlayerTopSpeed, PlayerTopSpeed), Player.velocity.y);
        }
    }

    // Checking Collisions
    bool CheckCollision(Vector2 position, Vector2 direction, float displacement, LayerMask definedObstacle)
    {
        return Physics2D.BoxCast(position, playerCollider.bounds.size, 0f, direction, displacement, definedObstacle);
    }

    // Generalized GIGACHAD Timers :)

    float ConditionalTimerDuration(float TimerCounter, float Duration, bool Condition)
    {
        if (Condition)
        {
            TimerCounter = Duration;
        }

        return TimerCounter;
    }

    float Timer(float TimerCounter)
    {
        TimerCounter -= Time.deltaTime;

        return TimerCounter;
    }

    bool CheckCondition(float TimerCounter, bool Condition)
    {
        if (TimerCounter > 0)
        {
            Condition = true;
        }
        else if (TimerCounter <= 0)
        {
            Condition = false;
        }

        return Condition;
    }

}
