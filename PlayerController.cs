using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //movement
    public Rigidbody2D theRB;
    public float moveSpeed;
    public float jumpForce;

    //jumping
    public Transform groundPoint;
    private bool isOnGround;
    public LayerMask whatIsGround;

    public Animator anim;

    //bullet
    public BulletController shotToFire;
    public Transform shotPoint;

    private bool canDoubleJump;

    public float dashSpeed, dashTime;
    private float dashCounter;

    public SpriteRenderer theSR, afterImage;
    public float afterImageLifetime, timeBetweenAfterImages;
    private float afterImageCounter;
    public Color afterImageColor;

    public float waitAfterDashing;
    private float dashRechargeCounter;

    public GameObject standing, ball;
    public float waitToBall;
    private float ballCounter;
    public Animator ballAnim;

    //bombs
    public Transform bombPoint;
    public GameObject bomb;

    //unlockable abilities
    private PlayerAbilityTracker abilities;

    public bool canMove;

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

        if(dashRechargeCounter > 0)
        {
            dashRechargeCounter -= Time.deltaTime;
        }
        else {

            if(Input.GetButtonDown("Fire2") && standing.activeSelf && abilities.canDash)
            {
                dashCounter = dashTime;

                ShowAfterImage();
            }
        }

        if(dashCounter > 0)
        {
            dashCounter = dashCounter - Time.deltaTime;

            theRB.velocity = new Vector2(dashSpeed * transform.localScale.x, theRB.velocity.y);

            afterImageCounter -= Time.deltaTime;
            if(afterImageCounter <= 0)
            {
                ShowAfterImage();
            }

            dashRechargeCounter = waitAfterDashing;
        } 
        else
        {

            //Get horizontal movement info
            theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, theRB.velocity.y);

            //handle direction change
            if(theRB.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            } else if (theRB.velocity.x > 0)
            {
                transform.localScale = Vector3.one;
            }
        }

        isOnGround = Physics2D.OverlapCircle(groundPoint.position, .2f, whatIsGround);

        //Get vertical movement info
        if(Input.GetButtonDown("Jump") && (isOnGround || (canDoubleJump && abilities.canDoubleJump)))
        {
            if(isOnGround)
            {
                canDoubleJump = true;

            } else
            {
                canDoubleJump = false;

                anim.SetTrigger("doubleJump");
            }

            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
        }

        //Fire bullets
        if(Input.GetButtonDown("Fire1"))
        {
            if(standing.activeSelf)
            {
            Instantiate(shotToFire, shotPoint.position, shotPoint.rotation).moveDir = new Vector2(transform.localScale.x, 0f);

            anim.SetTrigger("shotFired");
            } else if(ball.activeSelf && abilities.canDropBomb)
            {
                Instantiate(bomb, bombPoint.position, bombPoint.rotation);
            }
        }

        //ball mode
        if(!ball.activeSelf)
        {
            if(Input.GetAxisRaw("Vertical") < -.9f && abilities.canBecomeBall)
            {
                ballCounter -= Time.deltaTime;
                if(ballCounter <= 0)
                {
                    ball.SetActive(true);     
                    standing.SetActive(false);       
                }
            } else
            {
                ballCounter = waitToBall;
            }
        }
        else
        {
            if(Input.GetAxisRaw("Vertical") > .9f)
            {
                ballCounter -= Time.deltaTime;
                if(ballCounter <= 0)
                {
                    ball.SetActive(false);     
                    standing.SetActive(true);       
                }
            } else
            {
                ballCounter = waitToBall;
            }
        }
    } else 
        {
            theRB.velocity = Vector2.zero;
        }

        //get values for animator to determind which animation to use
        if(standing.activeSelf)
        {
            anim.SetBool("isOnGround", isOnGround);
            anim.SetFloat("speed", Mathf.Abs(theRB.velocity.x));
        }

        if(ball.activeSelf)
        {
            ballAnim.SetFloat("Speed", Mathf.Abs(theRB.velocity.x));
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
}
