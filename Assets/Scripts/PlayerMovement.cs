using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float playerSpeed = 7.0f;
    [SerializeField] float jumpSpeed = 20.0f;
    [SerializeField] float climbSpeed = 4.0f;
    [SerializeField] Vector2 deathSpeed = new Vector2 (20, 20);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip dead;

    bool isAlive = true;
    float gravityScaleAtStart;
    // Start is called before the first frame update
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator; 
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        myFeetCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive) { 
            Run();
            FlipPlayer();
            ClimpLadder();
            Die();
        }
        else
        {
            return;
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2 (moveInput.x * playerSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        bool playerHasSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRuning", playerHasSpeed);

    }

    void FlipPlayer()
    {
        if (myRigidbody.velocity.x > 0 || myRigidbody.velocity.x < 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { return; }


        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }


    void ClimpLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("IsClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("IsClimbing", playerHasVerticalSpeed);
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy","Hazards"))) {
            AudioSource.PlayClipAtPoint(dead, Camera.main.transform.position);
            isAlive = false;
            myAnimator.SetTrigger("Die");
            myRigidbody.velocity = deathSpeed;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }

    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
        AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }
}
