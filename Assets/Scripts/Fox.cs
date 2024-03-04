using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fox : MonoBehaviour
{
    //Public Fields
    [SerializeField] public float speed = 800;
    [SerializeField] public float jumpPower = 500;
    [SerializeField]Transform groundCheckCollider;
    const float groundCheckRadius = 0.02f;
    [SerializeField] LayerMask groundLayer;

    //Private Fields
    Rigidbody2D rb ;
    Animator animator; 
    float horizontalValue;
    float runSpeedModifier = 2f;
    [SerializeField] int totalJumps;
    int availableJumps;

    [SerializeField] bool isGrounded;
    bool Sprint;
    bool facingRight = true;
    bool multipleJump;
    bool cayoteJump;

    private void Awake()
    {
        availableJumps = totalJumps;

        rb = GetComponent<Rigidbody2D>();  
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //set the yVelocity in the animator
        animator.SetFloat("yVelocity", rb.velocity.y);

        //store the horizontal value
        horizontalValue = Input.GetAxisRaw("Horizontal");

        //if Lshift is clicked run
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint = true;
        }
        //if lshift is released disable run
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Sprint = false;
        }

        //if we pressed jump button enable jump 
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            
        }

 
    }

    private void FixedUpdate()
    {
        groundCheck();
        Move(horizontalValue);
    }

    void groundCheck()
    {
        bool wasGrounded = isGrounded;  
        isGrounded = false;
        //check if GroundCheckCollider is colliding with the other 2D  colliders that are in the "Ground" layer
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if(colliders.Length > 0)
        {
            isGrounded = true;
            if(!wasGrounded)
            {
                availableJumps = totalJumps;
                multipleJump = false; 
            }
        }
        else
        {
            if (wasGrounded)
            {
                StartCoroutine(CayoteJumpDelay());
            }
        }

        //as long as we are grounded the Jump tool is disabled in animator
        animator.SetBool("Jump", !isGrounded);
    }

    IEnumerator CayoteJumpDelay()
    {
        cayoteJump = true;
        yield return new WaitForSeconds(0.2f);
        cayoteJump = false;
    }

    void Jump()
    {
        //if the player is grounded and pressed space jump
        if (isGrounded)
        {          
            multipleJump = true;
            availableJumps--;

            rb.velocity = Vector2.up * jumpPower;
            animator.SetBool("Jump", true);
        }
        else
        {
            if (cayoteJump)
            {
                multipleJump = true;
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);
            }

            if (multipleJump && availableJumps > 0)
            {
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);
            }
        }
    }


    void Move(float dir)
    {
        

        #region Move & Run
        //Set value of x using dir and speed
        float xVal = dir * speed * 100 * Time.fixedDeltaTime;
        //If we are running multiply with the running modifier
        if(Sprint)
        {
            xVal*= runSpeedModifier;
        }
        //create Vec2 for teh velocity
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y); 
        //Set the player's velocity
        rb.velocity = targetVelocity;

        //if looking right and clicked left (flip to the left)
        if(facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-6, 6, 6);
            facingRight = false;
        }
        //if looking left and clicked right (flip to the right) 
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(6, 6, 6);
            facingRight = true;
        }

        //0 idle  , 4 walking    , 8 running
        //Set the float xVelocity according to the x value of the rigidbody2d velocity
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }
}
