using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolAI : MonoBehaviour
{
    // Player player;
    [SerializeField] BoxCollider2D coneOfVision;
    // [SerializeField] BoxCollider2D colisionDetction;

    // Coroutine firingCoroutine;
    //float jumpSpeed = 5.5f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePefab;
    [SerializeField] float projectileSpeed = 1700f;
    [SerializeField] float projectileFiringPeriod = 5f;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;

    float firingTimer = 0;
    enum PatrolState { patroling, shooting };
    PatrolState patrolState;
    Rigidbody2D rb;
    EnemyMovement enemyMovement;
    Animator animator;

    /* Vector2 jumpVelocityToAdd;
     bool isJumping = false;
     CircleCollider2D jumpColider;
     CapsuleCollider2D bodyCollider;*/

    //bool hasStarted = false;

    private void Awake()
    {
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
        patrolState = PatrolState.patroling;
        //player = FindObjectOfType<Player>();
        // jumpColider = GetComponent<CircleCollider2D>(); 
        // bodyCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        ProcesssState();

        if (coneOfVision.IsTouchingLayers(LayerMask.GetMask("Player", "PlayerInvader"))) patrolState = PatrolState.shooting;
    }
    void Fire()
    {
        if (coneOfVision.IsTouchingLayers(LayerMask.GetMask("Player", "PlayerInvader")) && Time.time > firingTimer)
        {
            GameObject projectile = Instantiate(projectilePefab, coneOfVision.transform.position, Quaternion.Euler(transform.eulerAngles)) as GameObject;
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(Time.deltaTime * projectileSpeed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180), 0);
            projectile.GetComponent<FireBall>().ProcessFieredShoot(coneOfVision.transform.position, transform.eulerAngles);
            //gameScessionObj.GetComponent<AudioSource>().PlayOneShot(shootAudioClips[Random.Range(0, shootAudioClips.Length)], volume);
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.ShootAudioClips[Random.Range(0, SFX.ShootAudioClips.Length)],
               SFX.ShootVolume * SFX.VolumeMultiplier(transform.position));
            firingTimer = Time.time + projectileFiringPeriod;
        }        
    }

    void ProcesssState()
    {
        switch (patrolState)
        {
            case PatrolState.shooting:
                Fire();
                enemyMovement.enabled = false;
                animator.SetBool("Shoot", true);
                rb.velocity = new Vector2(0, 0);
                break;
            case PatrolState.patroling:
                animator.SetBool("Shoot", false);
                enemyMovement.enabled = true;
                break;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Invader")
        {
            patrolState = PatrolState.patroling;
        }
    }
    /* private void FixedUpdate()
     {
         if (isJumping)
         {
             rb.AddForce(new Vector2(600f, 600f));
         }
         //GetComponent<Rigidbody2D>().velocity += jumpVelocityToAdd;
         //GetComponent<Rigidbody2D>().AddForce(jumpVelocityToAdd);

         //BoxCollider2D collider = GetComponent<BoxCollider2D>();

         //if (collider.IsTouchingLayers(LayerMask.GetMask("Ground")))
         //{
         //    isJumping = false;
         //}

          if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && isJumping)
          {
             // Debug.Log("fly");
             isJumping = false;

             // GetComponent<Rigidbody2D>().velocity += new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * 2f, jumpSpeed);
             //rb.AddForce(Vector2.up * 9600f);
          }

     }*/
    //private void FixedUpdate()
    //{
    //    Jump();
    //}

    /* private void SetJumpVelocity()
     {
         //if (!colisionDetction.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
         //    jumpVelocityToAdd = Vector2.zero;
         //    //colisionDetction.enabled = true;
         //    return;
         //}
         if (jumpColider.IsTouchingLayers(LayerMask.GetMask("Ground")))
         {
             //colisionDetction.enabled = false;
             Debug.Log("jump");
             isJumping = true;
             //jumpVelocityToAdd = new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180)*2f, jumpSpeed);
             //Debug.Log("speed 2: " + jumpVelocityToAdd);
             //GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * 2f, jumpSpeed));
             //GetComponent<Rigidbody2D>().velocity += jumpVelocityToAdd;
         }

     }*/

    //private void Jump()
    //{

    //}

    /* private void OnTriggerEnter2D(Collider2D collision)
     {
         SetJumpVelocity();
     }*/

    /* REFACTUARING
    void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {

            case "Forground":
                isJumping = true;
                break;

                /*case "SmallStump":
                    rb.AddForce(Vector2.up * 450f);
                    break;
        }
    }*/
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("colision");
    //}

    // FAZER UM SCRIPT PARA O OBJECTO QUE TEM O COLIDER

    /*private void test()
    {
        bool testJump = false;
        string other;

        // DO ON TRIGER
        if(other.tag == "jumper" && !testJump)
        {
            testJump = false;
            Vector2 forward = transform.parent.forward;
            GetComponentInParent<Rigidbody2D>().AddForce((forward + Vector2.up) * 1.0f, ForceMode2D.Impulse);
        }
    }*/
    //ForceMode2D.Implulse;
    //ForceMode2D.Force;
}
