using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    // Initializing Player Rigidbody
    private Rigidbody2D Player;

    // Initializing Player Collider
    private BoxCollider2D playerCollider;

    // Initializing Player sp
    private SpriteRenderer sp;

    // Memorizing original size of collider
    private Vector2 originalSize;

    // Obstacles
    [SerializeField] public LayerMask nonTraversableObstacle;
    [SerializeField] public LayerMask TraversableObstacle;
    [SerializeField] public LayerMask DeathLayer;
    [SerializeField] public LayerMask GrabbableLayer;

    //Box Collidiers
    public bool onGround;
    bool onCeilling;
    bool onLeftWall;
    bool onRightWall;
    bool onDeathLayer;

    // Keyboard Inputs
    private bool rightArrowButton;
    private bool leftArrowButton;
    private bool downArrowButton;

    private bool holdingJumpButton;
    private bool holdingDashButton;

    private bool pressedJumpButton;
    private bool pressedDashButton;

    private bool mouseLeftButton;

    public Vector2 mouseDirection;
    public float mouseDirectionX; // Detects in which direction the player is going discretly in the x-axis (-1, 0, 1)
    public float mouseDirectionY; // Detects in which direction the player is going discretly in the y-axis (-1, 0, 1)

    // Gravity
    [SerializeField] private float gravityAccUp = 25f;
    [SerializeField] private float gravityAccDown = 75f;
    [SerializeField] private float gravityAccApoge = 12.5f;
    [SerializeField] private float gravityAccAfterDash = 60f;

    // Jump Variables
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float numberOfJumps = 2f;
    [SerializeField] private float jumpCount;
    bool hasJumped;

    [SerializeField] private float coyoteTimeThreshold = 0.2f;
    private float coyoteTimeCounter;
    private bool canCoyote;

    [SerializeField] private float coyoteTimeDoubleThreshold = 0.2f;
    private float coyoteTimeDoubleCounter;
    private bool canCoyoteDouble;

    [SerializeField] private float jumpBufferTimeThreshold = 0.2f;
    private float jumpBufferTimeCounter;
    private bool canJumpBuffer;

    // Left-Right
    [SerializeField] private float accelerationFactor = 3f;
    [SerializeField] private float PlayerTopSpeed = 9f;
    [SerializeField] private float PowerCoefficient = 1.5f;
    float DirectionalValue;

    // Dash
    [SerializeField] float DashVelocity = 13f;

    [SerializeField] private float DashTime = 0.5f;
    private float DashTimeCounter;
    private bool canDash;
    private bool isDashing;
    private Vector2 DashDirection;
    private bool hasDashed;

    // Crouch
    [SerializeField] float TopCrouchSpeed = 5f;

    // Friction
    [SerializeField] float DecelerationForceGround = 10f;
    [SerializeField] float DecelerationForceAir = 3f;

    // Grab
    [SerializeField] private Transform grabPoint;
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDistance = 1f;

    private GameObject grabbedObject;

    // Glitches
    [SerializeField] bool NoBoundInSpeedGlitch;
    [SerializeField] bool MomentumGainGlitch;
    [SerializeField] bool CoyoteTimeGlitch;
    [SerializeField] bool DoubleJumpGlitch;
    [SerializeField] bool JumpDashMomentumGlitch;
    [SerializeField] bool PhaseThroughWallsGlitch;

    // Clipping / Collisions
    [SerializeField] float CanClipThroughSpeed = 50f;
    [SerializeField] float ClipDistance = 4f;
    [SerializeField] float AfterClipVelocity = 6f;

    // Jump-Dash Glitch
    bool JumpDashActive;
    [SerializeField] float DecelerationJumpDash = 0.5f;

    // Grab Glitch
    bool thrownObject;
    Vector2 thrownReaction;

    public static bool isDead;

    private GameMaster gm;

    [SerializeField] private GameObject loadingScreen;

    private Animator anim;

    public ParticleSystem jumpDust;

    public ParticleSystem stepDust;

    private bool spawnDust;

    RaycastHit2D Ground;

    [SerializeField] private AudioSource dashAudioSource;

    [SerializeField] private List<AudioClip> dashAudioClips;

    [SerializeField] private AudioSource deathAudioSource;

    [SerializeField] private ParticleSystem DashParticles;

    [SerializeField] private VisualEffect DashVFX;

    public bool canFlip;

    public bool canMove;


    // Start is called before the first frame update
    void Start()
    {
        // Initializing Rigidbody
        Player = GetComponent<Rigidbody2D>();

        // Initializing Player's Collider
        playerCollider = GetComponent<BoxCollider2D>();

        // Getting the Player's box collider original size
        originalSize = playerCollider.size;

        sp = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();

        transform.position = gm.lastCheckPointPos;

        canFlip = true;

        canMove = true;
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

        mouseLeftButton = Input.GetMouseButton(0);

        // The cursorPosition variable now contains the position of the cursor in screen coordinates
        Vector2 cursorPosition = Input.mousePosition;

        // The mouseDirection variable now contains the position of the cursor in world coordinates
        mouseDirection = (new Vector2(Camera.main.ScreenToWorldPoint(cursorPosition).x, Camera.main.ScreenToWorldPoint(cursorPosition).y) - Player.position).normalized;
        float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

        // Ensure angle is between 0 and 2pi
        while (angle < 0) angle += 2 * Mathf.PI;
        while (angle >= 2 * Mathf.PI) angle -= 2 * Mathf.PI;

        // Quantize direction based on angle
        if (angle >= 15 * Mathf.PI / 8 || angle < Mathf.PI / 8) // Right
        {
            mouseDirectionX = 1;
            mouseDirectionY = 0;
        }
        else if (angle >= Mathf.PI / 8 && angle < 3 * Mathf.PI / 8) // Up-Right
        {
            mouseDirectionX = 1;
            mouseDirectionY = 1;
        }
        else if (angle >= 3 * Mathf.PI / 8 && angle < 5 * Mathf.PI / 8) // Up
        {
            mouseDirectionX = 0;
            mouseDirectionY = 1;
        }
        else if (angle >= 5 * Mathf.PI / 8 && angle < 7 * Mathf.PI / 8) // Up-Left
        {
            mouseDirectionX = -1;
            mouseDirectionY = 1;
        }
        else if (angle >= 7 * Mathf.PI / 8 && angle < 9 * Mathf.PI / 8) // Left
        {
            mouseDirectionX = -1;
            mouseDirectionY = 0;
        }
        else if (angle >= 9 * Mathf.PI / 8 && angle < 11 * Mathf.PI / 8) // Down-Left
        {
            mouseDirectionX = -1;
            mouseDirectionY = -1;
        }
        else if (angle >= 11 * Mathf.PI / 8 && angle < 13 * Mathf.PI / 8) // Down
        {
            mouseDirectionX = 0;
            mouseDirectionY = -1;
        }
        else // Down-Right
        {
            mouseDirectionX = 1;
            mouseDirectionY = -1;
        }

        // Detect Trivial Collisions
        isGrounded();
        onCeilling = CheckCollision(playerCollider.bounds.center, Vector2.up, 0.1f, nonTraversableObstacle);
        onLeftWall = CheckCollision(playerCollider.bounds.center, Vector2.left, 0.1f, nonTraversableObstacle);
        onRightWall = CheckCollision(playerCollider.bounds.center, Vector2.right, 0.1f, nonTraversableObstacle);

        onDeathLayer = CheckCollision(playerCollider.bounds.center, Vector2.down, 0.1f, DeathLayer);

        // Coyote Time
        coyoteTimeCounter = ConditionalTimerDuration(coyoteTimeCounter, coyoteTimeThreshold, onGround);
        coyoteTimeCounter = Timer(coyoteTimeCounter);
        canCoyote = CheckCondition(coyoteTimeCounter, canCoyote);

        // Coyote Time Double
        coyoteTimeDoubleCounter = ConditionalTimerDuration(coyoteTimeDoubleCounter, coyoteTimeDoubleThreshold, onGround || jumpCount < numberOfJumps);
        coyoteTimeDoubleCounter = Timer(coyoteTimeDoubleCounter);
        canCoyoteDouble = CheckCondition(coyoteTimeDoubleCounter, canCoyoteDouble);

        // Jump Buffer Time
        jumpBufferTimeCounter = ConditionalTimerDuration(jumpBufferTimeCounter, jumpBufferTimeThreshold, pressedJumpButton);
        jumpBufferTimeCounter = Timer(jumpBufferTimeCounter);
        canJumpBuffer = CheckCondition(jumpBufferTimeCounter, canJumpBuffer);

        // Dash
        if (onGround && !holdingDashButton)
        {
            canDash = true;
            hasDashed = false;
        }
        // Jump-Dash Glitch
        if (onGround)
        {
            JumpDashActive = false;
        }

        DashTimeCounter = Timer(DashTimeCounter);
        isDashing = CheckCondition(DashTimeCounter, isDashing);

        anim.SetBool("isRunning", leftArrowButton || rightArrowButton);
        anim.SetBool("grounded", onGround);
        anim.SetBool("isFalling", Player.velocity.y < 0 && !onGround);
        anim.SetBool("isRising", Player.velocity.y > 0 && !onGround);

        if (onDeathLayer && !isDead)
        {
            StartCoroutine(Death(0.6f));
        }
            
    }

    /*void OnDrawGizmos()
    {
        // Cast size, which should be same as the player's bounding box size
        Vector2 castSize = playerCollider.bounds.size;
        float displacement = 0.1f;

        // Set Gizmos color
        Gizmos.color = Color.red;

        // Draw the ground collision
        Gizmos.DrawWireCube((Vector2)playerCollider.bounds.center + Vector2.down * displacement, castSize);

        // Draw the ceiling collision
        Gizmos.DrawWireCube((Vector2)playerCollider.bounds.center + Vector2.up * displacement, castSize);

        // Draw the left wall collision
        Gizmos.DrawWireCube((Vector2)playerCollider.bounds.center + Vector2.left * displacement, castSize);

        // Draw the right wall collision
        Gizmos.DrawWireCube((Vector2)playerCollider.bounds.center + Vector2.right * displacement, castSize);
    }*/

    // Fixed Update is called every interval
    void FixedUpdate()
    {
        // Get Direction Player wants to go
        if (leftArrowButton && !rightArrowButton)
        {
            DirectionalValue = -1;

            if (canFlip)
            {
                //Flips the sprite
                sp.flipX = true;
            }
     
        }
        else if (!leftArrowButton && rightArrowButton)
        {
            DirectionalValue = 1;

            if (canFlip)
            {
                //Flips the sprite
                sp.flipX = false;
            }
        }
        else
        {
            DirectionalValue = 0;
        }

        if (canMove)
        {
            // Gravity
            GravityPhysics();

            // Jump
            JumpPhysics();

            // Left-Right Movement
            DirectionalPhysics();

            // Dash
            DashPhysics();

            // Crouch
            CrouchPhysics();

            // Friction
            FrictionPhysics();

            // Grabbed Object
            GrabPhysics();

            // Glitch Through Walls
            WallGlitch();

            // Glitch Through walls with object
            GrabGlitch();
        }

    }

    // Gravity Physics
    void GravityPhysics()
    {
        if (!isDashing)
        {
            if (hasDashed)
            {
                Player.AddForce(Vector2.down * gravityAccAfterDash);
            }
            else if (Mathf.Abs(Player.velocity.y) < 0.7f && holdingJumpButton)
            {
                Player.AddForce(Vector2.down * gravityAccApoge);
            }
            else if (Player.velocity.y > 0 && holdingJumpButton)
            {
                Player.AddForce(Vector2.down * gravityAccUp);
            }
            else
            {
                Player.AddForce(Vector2.down * gravityAccDown);
            }
        }
    }

    // Jump Physics
    void JumpPhysics()
    {
        if (canCoyote && canJumpBuffer && !hasJumped)
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

            // Setting isDashing to false
            hasDashed = false;

            // Setting hasJumped to true
            hasJumped = true;
        }

        if (Player.velocity.y < 0)
        {
            hasJumped = false;
        }
    }
    void DoubleJumpPhysics()
    {
        if (canCoyoteDouble && canJumpBuffer && !hasJumped)
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

            // Setting isDashing to false
            hasDashed = false;

            // Setting hasJumped to true
            hasJumped = true;

            // Increment the jump count
            jumpCount++;
        }

        // Reset DoubleJump
        if (onGround && Player.velocity.y < 0.5f)
        {
            jumpCount = 0f;
        }

        if (Player.velocity.y < 0)
        {
            hasJumped = false;
        }
    }

    // Left And Right Physics
    void DirectionalPhysics()
    {
        if (DirectionalValue != 0)
        {
            // Calculate the acceleration based on the difference between current velocity and top velocity
            float acceleration = Mathf.Pow(Mathf.Abs(DirectionalValue * PlayerTopSpeed - Player.velocity.x), PowerCoefficient) * accelerationFactor;

            // Apply the acceleration to the rigidbody's velocity
            Player.AddForce(new Vector2(DirectionalValue, 0) * acceleration, ForceMode2D.Force);

            // Limit the velocity to the top speed
            if (downArrowButton && onGround)
            {
                Player.velocity = new Vector2(Mathf.Clamp(Player.velocity.x, -TopCrouchSpeed, TopCrouchSpeed), Player.velocity.y);
            }
            else
            {
                Player.velocity = new Vector2(Mathf.Clamp(Player.velocity.x, -PlayerTopSpeed, PlayerTopSpeed), Player.velocity.y);
            }
        }
    }
    void DirectionalPhysicsNoSpeedCap()
    {
        if (DirectionalValue != 0)
        {
            float currentVelocityMagnitude = Player.velocity.magnitude;

            // Check if the player has reached the top speed in the current direction
            bool isAtTopSpeed = Mathf.Abs(currentVelocityMagnitude) >= PlayerTopSpeed;

            // Check if the player is trying to accelerate in the same direction and has reached the top speed
            bool isAcceleratingInSameDirection = Mathf.Sign(Player.velocity.x) == Mathf.Sign(DirectionalValue);

            if (isAcceleratingInSameDirection && isAtTopSpeed)
            {
                // Do not apply any acceleration
                return;
            }

            // Calculate the acceleration based on the difference between current velocity and top velocity
            float acceleration = Mathf.Pow(Mathf.Abs(DirectionalValue * PlayerTopSpeed - Player.velocity.x), PowerCoefficient) * accelerationFactor;

            // Apply the acceleration to the rigidbody's velocity
            Player.AddForce(new Vector2(DirectionalValue, 0) * acceleration, ForceMode2D.Force);
        }
    }
    void DirectionalPhysicsNoSpeedBounds()
    {
        if (DirectionalValue != 0)
        {
            // Calculate the acceleration based on the difference between current velocity and top velocity
            float acceleration = Mathf.Pow(Mathf.Abs(DirectionalValue * PlayerTopSpeed - Player.velocity.x), PowerCoefficient) * accelerationFactor;

            // Apply the acceleration to the rigidbody's velocity
            Player.AddForce(new Vector2(DirectionalValue, 0) * acceleration, ForceMode2D.Force);

            // If on air, apply more force
            if (!onGround)
            {
                Player.AddForce(new Vector2(DirectionalValue, 0) * 5, ForceMode2D.Force);
            }
        }
    }
    // Dash
    void DashPhysics()
    {
        if (holdingDashButton && canDash)
        {
            //Dash SFX
            AudioClip dashClip = dashAudioClips[Random.Range(0, dashAudioClips.Count)];
            dashAudioSource.clip = dashClip;
            dashAudioSource.Play();

            //Dash VFX
            DashParticles.Play();
            DashVFX.Play();

            DashDirection = new Vector2(mouseDirectionX, mouseDirectionY).normalized;
            DashTimeCounter = ConditionalTimerDuration(DashTimeCounter, DashTime, true);
            canDash = false;
        }

        if (isDashing)
        {
            Player.velocity = DashDirection * DashVelocity;
            hasDashed = true;
        }
    }

    // Crouch
    void CrouchPhysics()
    {
        if (downArrowButton && CheckCollision(playerCollider.bounds.center, Vector2.down, 0.3f, nonTraversableObstacle))
        {
            // When down key is pressed, reduce the height of the box collider
            playerCollider.size = new Vector2(originalSize.x, originalSize.y / 2);
        }
        else
        {
            // When down key is released, return the box collider to its original size
            playerCollider.size = originalSize;
        }
    }

    // Friction

    void FrictionPhysics()
    {
        if (!leftArrowButton && !rightArrowButton)
        {
            if (onGround)
            {
                Player.AddForce(-new Vector2(Player.velocity.x, 0) * DecelerationForceGround);
            }
            else
            {
                Player.AddForce(-new Vector2(Player.velocity.x, 0) * DecelerationForceAir);
            }

            if (Mathf.Abs(Player.velocity.x) <= 1)
            {
                Player.velocity = new Vector2(0, Player.velocity.y);
            }
        }
    }

    void JumpDashFrictionPhysics()
    {
        if (!leftArrowButton && !rightArrowButton)
        {
            if (onGround && !JumpDashActive)
            {
                Player.AddForce(-new Vector2(Player.velocity.x, 0) * DecelerationForceGround);
            }
            else if (!JumpDashActive)
            {
                Player.AddForce(-new Vector2(Player.velocity.x, 0) * DecelerationForceAir);
            }
            else
            {
                Player.AddForce(-new Vector2(Player.velocity.x, 0) * DecelerationJumpDash);
            }

            if (Mathf.Abs(Player.velocity.x) <= 1)
            {
                Player.velocity = new Vector2(0, Player.velocity.y);
            }
        }
    }

    // Grabbing
    private void GrabPhysics()
    {
        RaycastHit2D hitInfo;

        if (Player.velocity.x != 0)
        {
            hitInfo = Physics2D.Raycast(rayPoint.position, new Vector3(DirectionalValue, 0, 0), rayDistance, GrabbableLayer);
        }
        else
        {
            hitInfo = Physics2D.Raycast(rayPoint.position, new Vector3(transform.localScale.x, 0, 0), rayDistance, GrabbableLayer);
        }

        if (hitInfo.collider != null)
        {
            // Grab Object
            if (mouseLeftButton)
            {
                grabbedObject = hitInfo.collider.gameObject;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.position = grabPoint.position;
                grabbedObject.transform.SetParent(transform);
            }
            else if (!mouseLeftButton && grabbedObject != null)
            {
                Rigidbody2D grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody2D>();
                grabbedObjectRigidbody.isKinematic = false;

                if (!(downArrowButton && onGround))
                {
                    Vector2 throwDirection = new Vector2(mouseDirectionX, mouseDirectionY).normalized;

                    // Adjust player velocity to prevent obstruction of object repelling
                    if (Mathf.Sign(Player.velocity.x) == Mathf.Sign(throwDirection.x))
                    {
                        Player.velocity = new Vector2(0f, Player.velocity.y);
                    }

                    if (Mathf.Sign(Player.velocity.y) == Mathf.Sign(throwDirection.y))
                    {
                        Player.velocity = new Vector2(Player.velocity.x, 0f);
                    }

                    // Throw the object
                    grabbedObjectRigidbody.velocity = PolarVector(15f, Mathf.Atan2(mouseDirectionY, mouseDirectionX));

                    // Apply a force to the player in the opposite direction
                    float throwForceMagnitude = 15f;
                    Vector2 playerThrowForce = PolarVector(-0.5f * throwForceMagnitude, Mathf.Atan2(mouseDirectionY, mouseDirectionX));
                    Player.AddForce(playerThrowForce, ForceMode2D.Impulse);

                    // Detect the throw
                    thrownObject = true;
                    thrownReaction = playerThrowForce;
                }

                hasDashed = false;
                grabbedObject.transform.SetParent(null);
                grabbedObject = null;
            }
        }
    }

    // Wall Glitch (CHANGE FROM NONTRAVERSABLE TO TRAVERABLE ONCE THIS HAS BEEN IMPLEMENTED)
    void WallGlitch()
    {
        if (Player.velocity.magnitude > CanClipThroughSpeed && CheckRay(playerCollider.bounds.center, Player.velocity.normalized, 0.7f, nonTraversableObstacle))
        {
            Vector2 ClipPosition = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.center.y) + Player.velocity.normalized * ClipDistance;

            while (!CheckCollision(ClipPosition - Player.velocity.normalized * Time.fixedDeltaTime, Vector2.zero, 0f, nonTraversableObstacle))
            {
                ClipPosition -= Player.velocity.normalized * Time.fixedDeltaTime;
            }

            ClipPosition += Player.velocity.normalized * Time.fixedDeltaTime;

            if (!CheckCollision(ClipPosition, Vector2.zero, 0f, nonTraversableObstacle))
            {
                transform.position = ClipPosition;
                Player.velocity = Player.velocity.normalized * AfterClipVelocity;
            }
        }
    }

    // Grab Glitch
    void GrabGlitch()
    {
        if (thrownObject && CheckRay(playerCollider.bounds.center, new Vector2(thrownReaction.x, 0).normalized, 0.7f, nonTraversableObstacle) && onGround)
        {
            Vector2 ClipPosition = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.center.y + 0.1f) + new Vector2(thrownReaction.x, 0).normalized * ClipDistance;

            while (!CheckCollision(ClipPosition - new Vector2(thrownReaction.x, 0).normalized * Time.fixedDeltaTime, Vector2.zero, 0f, nonTraversableObstacle))
            {
                ClipPosition -= new Vector2(thrownReaction.x, 0).normalized * Time.fixedDeltaTime;
            }

            ClipPosition += new Vector2(thrownReaction.x, 0).normalized * Time.fixedDeltaTime;

            if (!CheckCollision(ClipPosition, Vector2.zero, 0f, nonTraversableObstacle))
            {
                transform.position = ClipPosition;
                Player.velocity = new Vector2(thrownReaction.x, 0).normalized * AfterClipVelocity;
            }
            else
            {
                thrownObject = false;
                thrownReaction = Vector2.zero;
            }
        }
        else
        {
            thrownObject = false;
            thrownReaction = Vector2.zero;
        }
    }
    public void isGrounded() // GROUND CHECK
    {
        Ground = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, .2f, nonTraversableObstacle);;

        if (Ground)
        {
            onGround = true;

            if (spawnDust)
            {
                GetComponent<FootStepsManager>().SelectAndPlayFootstep();
                spawnDust = false;
                jumpDust.transform.position = Ground.point;
                jumpDust.transform.parent = null;
                jumpDust.Play();

            }
        }
        else
        {
            spawnDust = true;
            onGround = false;
        }

    }

    // Checking Collisions
    bool CheckCollision(Vector2 position, Vector2 direction, float displacement, LayerMask definedObstacle)
    {
        return Physics2D.BoxCast(position, playerCollider.bounds.size, 0f, direction, displacement, definedObstacle);
    }

    // Checking Raycasts
    bool CheckRay(Vector2 position, Vector2 direction, float distance, LayerMask definedObstacle)
    {
        return Physics2D.Raycast(position, direction, distance, definedObstacle);
    }

    // Checking Point
    public bool CheckPointCollision(Vector2 point, LayerMask layerMask)
    {
        Collider2D collider = Physics2D.OverlapPoint(point, layerMask);
        return collider != null;
    }

    // Angeled Vector
    Vector2 PolarVector(float magnitude, float angle)
    {
        return magnitude * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
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
    
    //Called in the Animator
    public void playStepParticles()
    {
        stepDust.transform.position = Ground.point;
        stepDust.transform.parent = null;
        stepDust.Play();
    }
    public IEnumerator Death(float time)
    {
        anim.SetTrigger("Dead");

        deathAudioSource.Play();

        isDead = true;

        canMove = false;

        canFlip = false;

        Player.simulated = false;

        yield return new WaitForSeconds(time);

        transform.position = gm.lastCheckPointPos;

        isDead = false;

        canMove = true;

        canFlip = true;

        Player.simulated = true;

        loadingScreen.SetActive(false);
    }
}