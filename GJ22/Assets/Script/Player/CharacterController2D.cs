using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] private float m_JumpForceHorizontalFromWall = 200f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [Range(0, 1)] [SerializeField] private float m_WallFallSpeed;
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_WallCheck;
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded { get; private set; }        // Whether or not the player is grounded.
    const float k_WallRadius = .1f;
    public bool m_Wall { get; private set; }
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    public Rigidbody2D m_Rigidbody2D { get; private set; }
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    private float waitForGrounded = 0.1f;
    private bool jumpedFromWall;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;

    public UnityEvent OnWallEvent;
    public UnityEvent NotOnWallEvent;

    private bool m_wasCrouching = false;

    private void Awake()
    {

        
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        if (OnWallEvent == null)
            OnWallEvent = new UnityEvent();

        if (NotOnWallEvent == null)
            NotOnWallEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        bool wasWall = m_Wall;
        m_Wall = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] collidersGround = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        Collider2D[] collidersWall = Physics2D.OverlapCircleAll(m_WallCheck.position, k_WallRadius, m_WhatIsGround);
        if (waitForGrounded > 0f)
        {
            waitForGrounded -= Time.fixedDeltaTime;
        }
        else
            for (int i = 0; i < collidersGround.Length; i++)
            {
                if (collidersGround[i].gameObject != gameObject && collidersGround[i].isTrigger == false)
                {
                    m_Grounded = true;
                    if (!wasGrounded)
                        OnLandEvent?.Invoke();
                }
            }
        for (int i = 0; i < collidersWall.Length; i++)
        {
            if (collidersWall[i].gameObject != gameObject && collidersWall[i].isTrigger == false && (Input.GetAxis("Horizontal") != 0 || wasWall))
            {
                m_Wall = true;
                if (!wasWall && !m_Grounded)
                    OnWallEvent?.Invoke();
                //if (m_Rigidbody2D.velocity.x > 1 || m_Rigidbody2D.velocity.x < -1)


            }
        }
        if (!m_Wall && wasWall)
            NotOnWallEvent?.Invoke();
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }
        if ((m_Grounded || m_Wall) && jump)
        {
            waitForGrounded = 0.1f;
            // Add a vertical force to the player.
            if (m_Grounded)
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            else
            {
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                jumpedFromWall = true;
                NotOnWallEvent?.Invoke();
            }

            m_Grounded = false;
            m_Wall = false;
        }
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent?.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent?.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            // And then smoothing it out and applying it to the character
            if (jumpedFromWall)
            {
                move = -transform.localScale.x * m_JumpForceHorizontalFromWall;
                m_Rigidbody2D.velocity = Vector3.Scale(m_Rigidbody2D.velocity, new Vector3(0f, 1f, 1f));
                jumpedFromWall = false;
            }

            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            // If the input is moving the player right and the player is facing left...
            
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        if (m_Wall)
        {
            Vector3 targetFallSpeed = new Vector2(0f, m_WallFallSpeed * 10f);
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetFallSpeed, ref m_Velocity, m_MovementSmoothing);
        }

    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}