using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{


    //gravity scale = 1.75f
    public Rigidbody2D theRB;

    public ParticleSystem jumpParticle, slideParticle;
    private bool isFacingRight = true;
    private float horizontal, vertical;

    public float moveSpeed;
    private float jumpForce = 10f;
    private float climbTime = 2f;
    private float climbCounter;
    private bool hasDoubleJumped;

    //disable constant jumping
    public Transform groundPoint;
    private bool isOnGround;
    private bool wallGrab;
    private bool lastTouchedWallIsLeft, lastTouchedWallIsRight;
    private bool secondLastTouchedWallIsLeft, secondLastTouchedWallIsRight;

    //in testing for wall jumps and climbs --------------
    private bool isOnWall, isOnRightWall, isOnLeftWall;

    public LayerMask whatIsGround;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    //0, -0.15,     0.2, 0,         -0.2,0

    public float collisionRadius = 0.25f;

    public float current_slide_speed = 1f;
    public bool canWallJump, isWallJumping, isWallSliding;

    private float wallJumpTime = 0.3f;
    private float wallJumpCounter;
    private float wallJumpRechargeCounter;

    //
    public Animator anim;

    private bool canDoubleJump, canDash;
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private bool isDashingForward, isDashingDown, isDashingUp;
    private float dashCounter;
    private Vector2 m_DashDirection = Vector2.zero;

    public SpriteRenderer theSR, afterImage;
    public float afterImageLifetime, timeBetweenAfterImages;
    private float afterImageCounter;
    public Color afterImageColor;

    public float waitAfterDashing;
    private float dashRechargeCounter = 0.5f;

    public GameObject standing;

    //unlockable abilities
    private PlayerAbilityTracker abilities;

    public bool canMove;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
    }

    // Start is called before the first frame update
    void Start()
    {
        abilities = GetComponent<PlayerAbilityTracker>();

        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove && Time.timeScale != 0)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if(isOnGround)
            {
                canDash = true;
                hasDoubleJumped = false;
                climbCounter = climbTime;
            }

            ProcessDashing();

            //Ground and wall checks, ground -0.45, right 0.4, left -0.4
            isOnGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, whatIsGround);
            isOnRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, whatIsGround);
            isOnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, whatIsGround);
            isOnWall = isOnRightWall || isOnLeftWall;
            if (isOnGround)
                isOnWall = false;

            if(isOnRightWall)
            {
                lastTouchedWallIsRight = true;
                lastTouchedWallIsLeft = false;
            } else if(isOnLeftWall)
            {
                lastTouchedWallIsRight = false;
                lastTouchedWallIsLeft = true;
            }

            //Get vertical movement info
            if((isOnGround || (canDoubleJump && abilities.canDoubleJump && !isOnWall)))
            {
                if(isOnGround && Input.GetButtonDown("Jump"))
                {
                    canDoubleJump = true;
                    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                    hasDoubleJumped = false;
                } else if(Input.GetButtonDown("Jump"))
                {
                    canDoubleJump = false;

                    anim.SetTrigger("doubleJump");
                    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                    hasDoubleJumped = true;
                }
            }

            //wallGrabbing - y axis freezes when wall grabbed
            wallGrab = isOnWall && Input.GetKey(KeyCode.LeftShift) && !isOnGround && abilities.canWallClimb && climbCounter > 0;

            //wallSliding
            if (isOnWall && !isOnGround && !wallGrab && horizontal != 0)
            {
                if(climbCounter > 0)
                {
                    isWallSliding = true;
                }else
                {
                    isWallSliding = false;
                }
            }
            else
            {
                isWallSliding = false;
            }
             

            
            if(wallGrab)
            {
                //Player wall climbs faster when going downwards
                if(vertical == 1 && horizontal != 0)
                {
                    climbCounter = climbCounter - Time.deltaTime;
                    theRB.velocity = new Vector2(0f, vertical * moveSpeed * 0.5f);
                }
                else if(vertical == 1 && horizontal == 0)
                {
                    climbCounter = climbCounter - Time.deltaTime;
                    theRB.velocity = new Vector2(0f, Input.GetAxisRaw("Vertical") * moveSpeed * 0.5f);
                }
                else
                {
                    theRB.velocity = new Vector2(0f, Input.GetAxisRaw("Vertical") * moveSpeed);
                }
                theRB.gravityScale = 0;
            }
            else
            {
                if (isWallSliding)
                {
                    theRB.velocity = new Vector2(theRB.velocity.x, -current_slide_speed);
                }

                theRB.gravityScale = 2f;
            }

            if(climbCounter <= climbTime/2f)
            {
                //INSERT ANIM HERE OF FLASHING CHARACTER MODEL
            }



            if (isOnWall && (Input.GetButtonDown("Jump")))
            {
                wallJumpCounter = wallJumpTime;
                isWallJumping = true;
            }
            
            if(Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset * 2, collisionRadius, whatIsGround))
            {
                //DO NOTHING
                wallJumpCounter = 0;
            }
            else if(wallJumpCounter > 0)
            {
                wallJumpCounter = wallJumpCounter - Time.deltaTime;

                if (lastTouchedWallIsLeft)
                {
                    theRB.velocity = new Vector2(moveSpeed, moveSpeed);
                }
                else if (lastTouchedWallIsRight)
                {
                    theRB.velocity = new Vector2(-moveSpeed, moveSpeed);
                }
            }   
                
            if(theRB.velocity.y <= 0f)
            {
                isWallJumping = false;
            }
            

            

            standing.SetActive(true);
        } else
        {
            theRB.velocity = Vector2.zero;
        }
         //get values for animator to determind which animation to use
        if(standing.activeSelf)
        {
            anim.SetBool("isOnGround", isOnGround);
            anim.SetBool("isWallSliding", isWallSliding);
            anim.SetBool("isWallJumping", isWallJumping);
            anim.SetBool("wallGrab", wallGrab);
            anim.SetFloat("speed", Mathf.Abs(theRB.velocity.x));
            anim.SetFloat("verticalSpeed",theRB.velocity.y);
            anim.SetBool("isDashingForward", isDashingForward);
            anim.SetBool("isDashingDown", isDashingDown);
            anim.SetBool("isDashingUp", isDashingUp);
            anim.SetBool("hasDoubleJumped", hasDoubleJumped);


        }
    }



    private void ProcessDashing()
    {
        //Dashing
        if (dashRechargeCounter > 0)
        {
            dashRechargeCounter -= Time.deltaTime;
        }
        else
        {
            if (Input.GetButtonDown("Fire2") && (canDash && abilities.canDash))
            {
                // Only not dash if pointing down on the ground
                if (vertical == -1 && horizontal == 0 && isOnGround)
                {
                    //DO NOTHING
                } else if(isOnLeftWall && horizontal == -1)
                {
                    //DO NOTHING
                } else if(isOnRightWall && horizontal == 1)
                {
                    //DO NOTHING
                }
                else
                {
                    dashCounter = dashTime;
                    m_DashDirection = new Vector2(horizontal, vertical);
                    if (isOnGround)
                    {
                        canDash = true;
                    }

                    ShowAfterImage();
                }
            }
        }

        if (dashCounter > 0)
        {
            dashCounter = dashCounter - Time.deltaTime;            

            if (m_DashDirection != Vector2.zero)
            {
                if (m_DashDirection.x == 0f)
                {
                    theRB.velocity = new Vector2(0, m_DashDirection.y * dashSpeed * 0.5f); // When you have vertical input
                    if(m_DashDirection.y == 1f)
                    {
                        isDashingUp = true;
                    } else if(m_DashDirection.y == 0f)
                    {
                        isDashingDown = true;
                    }
                }else if ((m_DashDirection.x >= 0.25f || m_DashDirection.x <= -0.25f) && (m_DashDirection.y < 0.25f && m_DashDirection.y > -0.25f))
                {
                    theRB.velocity = new Vector2(m_DashDirection.x * dashSpeed, 0); // When you have horizontal input
                    isDashingForward = true;
                }
                else{ 
                    if((m_DashDirection.x >= 0.25f && m_DashDirection.y >= 0.25f))
                    {
                        theRB.velocity = new Vector2(1 * dashSpeed * 0.5f, 1 * dashSpeed * 0.5f); // When you have vert and hor input
                    }
                    else if((m_DashDirection.x >= 0.25f && m_DashDirection.y <= -0.25f))
                    {
                        theRB.velocity = new Vector2(1 * dashSpeed * 0.5f, -1 * dashSpeed * 0.5f); // When you have vert and hor input
                    }
                    else if((m_DashDirection.x <= -0.25f && m_DashDirection.y >= 0.25f))
                    {
                        theRB.velocity = new Vector2(-1 * dashSpeed * 0.5f, 1 * dashSpeed * 0.5f); // When you have vert and hor input
                    }
                    else if((m_DashDirection.x <= -0.25f && m_DashDirection.y  <= -0.25f))
                    {
                        theRB.velocity = new Vector2(-1 * dashSpeed * 0.5f, -1 * dashSpeed * 0.5f); // When you have vert and hor input
                    }


                    if(m_DashDirection.y == 1)
                    {
                        isDashingUp = true;
                    } else if(m_DashDirection.y == -1)
                    {
                        isDashingDown = true;
                    }
                }
                   
            }else
            {
                // if no movement input dash the way you face
                isDashingForward = true;
                if (isFacingRight)
                {
                    theRB.velocity = new Vector2(dashSpeed, 0f);
                }else
                {
                    theRB.velocity = new Vector2(-dashSpeed, 0f);
                }
            }


            afterImageCounter -= Time.deltaTime;
            if (afterImageCounter <= 0)
            {
                ShowAfterImage();
            }

            dashRechargeCounter = waitAfterDashing;
            canDash = false;





            if (hasDoubleJumped == true)
            {
                canDoubleJump = false;
            }

        }
        else
        {
            //Get horizontal movement info
            isDashingUp = false;
            isDashingDown = false;
            isDashingForward = false;


            theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, theRB.velocity.y);


            //handle direction change
            if (theRB.velocity.x > 0)
            {
                if(isWallSliding)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                } else{
                    transform.localScale = Vector3.one;
                }
                isFacingRight = true;
            }
            else if (theRB.velocity.x < 0)
            {
                if(isWallSliding)
                {
                    transform.localScale = Vector3.one;
                } else{
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                isFacingRight = false;
            }

            if(wallGrab && isOnLeftWall)
            {
                transform.localScale = Vector3.one;
            } else if(wallGrab && isOnRightWall)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "RegainDash")
        {
            canDash = true;
            dashCounter = 0;
            dashRechargeCounter = 0;
        }
    }

    public void ShowAfterImage()
    {
        SpriteRenderer image = Instantiate(afterImage, transform.position, transform.rotation);
        image.sprite = theSR.sprite;
        image.transform.localScale = transform.localScale;
        image.color = afterImageColor;

        Destroy(image.gameObject, afterImageLifetime);

        afterImageCounter = timeBetweenAfterImages;
    }

    private void WallSlide()
    {
        if(horizontal != 0f)
        {   
            isWallSliding = true;
            theRB.velocity = new Vector2(theRB.velocity.x, -current_slide_speed);
        }
        else
            isWallSliding = false;
    }
}
