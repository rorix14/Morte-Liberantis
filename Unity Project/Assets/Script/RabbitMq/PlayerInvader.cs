using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvader : MonoBehaviour
{
    float runSpeed = 6f;
    float jumpSpeed = 28f;
    bool isCrouched = false;
    Rigidbody2D myRigidbody2D;
    Vector3 eulerAngles;
    Animator myAnimator;
    BoxCollider2D myFeet;
    [SerializeField] GameObject projectilePefab;
    float porjectileSpeed = 1200f;
    float projectileFiringPeriod = 0.3f;
    [SerializeField] GameObject fieredEffect;
    [SerializeField] GameObject melleAttack;
    float attackDoration = 0f;
    [SerializeField] GameObject dashEffect;
    bool doDash;
    [Header("Coliders")]
    [SerializeField] CapsuleCollider2D crouchCollider;
    [SerializeField] BoxCollider2D headCollider;
    CapsuleCollider2D myBodyColider2D;
    float waitForRecover = 0f;
    float recoverCoolDown = 0.5f;
    [SerializeField] GameObject healthBar;
    float currentHealth;
    float lastHealth;
    float moveX;
    float verticalAxis;
    string invaderInputs;
    float gravatiySatetAtStart;
    bool jumpInStaires;
    bool doJump;
    bool doDoubleJump;
    bool fire;
    bool doDeathKik;
    public float MoveX
    {
        get { return moveX; }
        set { moveX = value; }
    }
    public float VerticalAxis
    {
        get { return verticalAxis; }
        set { verticalAxis = value; }
    }
    public string InvaderInputs
    {
        set { invaderInputs = value; }
    }
    public float CurrentHealth
    {
        set { currentHealth = value; }
    }
    void Start()
    {
        if (DBManager.isInvader)
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        }

        myRigidbody2D = GetComponent<Rigidbody2D>();
        myFeet = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        myBodyColider2D = GetComponent<CapsuleCollider2D>();
        lastHealth = FindObjectOfType<MultiplayerProcessor>().GetComponent<MultiplayerProcessor>().LastPlayerHealth;
        gravatiySatetAtStart = myRigidbody2D.gravityScale;
    }

    void Update()
    {
        if (waitForRecover > 0)
        {
            waitForRecover -= Time.deltaTime;
            return;
        }

        if (attackDoration > 0)
        {
            if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                myRigidbody2D.velocity = Vector2.zero;
            }
            attackDoration -= Time.deltaTime;
            return;
        }
        Crouch();
        MoveInvader();
        ProssessInvaderInputs();
        Hurt();
        ClimingState();
    }
    private void FixedUpdate()
    {
        DoDash();
        Jump();
        JumpInStairs();
    }

    void MoveInvader()
    {
        Vector2 playerVelocity = new Vector2(moveX * runSpeed, myRigidbody2D.velocity.y);
        if (isCrouched)
        {
            myRigidbody2D.velocity = new Vector2((moveX * runSpeed) / 2, myRigidbody2D.velocity.y);
        }
        else myRigidbody2D.velocity = playerVelocity;

        if (moveX < 0)
        {
            eulerAngles.y = 180f;
           // healthBar.transform.eulerAngles;
        }
        else if (moveX > 0)
        {
            eulerAngles.y = 0;
        }
        else if (moveX == 0)
        {
            eulerAngles = transform.eulerAngles;
        }
        transform.eulerAngles = eulerAngles;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody2D.velocity.x) > Mathf.Epsilon;
        if (!isCrouched)
        {
            myAnimator.SetBool("Runing", playerHasHorizontalSpeed);
        }
        else
        {
            myAnimator.SetBool("Crouch Moving", playerHasHorizontalSpeed);
        }
    }

    void ProssessInvaderInputs()
    {
        float velocityX = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (porjectileSpeed * Time.deltaTime);
        switch (invaderInputs)
        {
            case "jump":
                doJump = true;
                break;
            case "doubleJump":
                DoubleJump();
                break;
            case "melle":
                attackDoration = 0.5f;
                melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionForward);
                StartCoroutine(melleAttack.GetComponent<MelleAttack>().ManangeInvaderMelle());
                break;
            case "dash":
                doDash = true;
                break;
            case "Look up":
                melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionUp);
                Fire(new Vector3(0, transform.eulerAngles.y, 90), new Vector2(0f, (porjectileSpeed * Time.deltaTime)));
                break;
            case "Angle up":
                melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionAngleUp);
                Fire(new Vector3(0, transform.eulerAngles.y, 45), new Vector2(velocityX, (porjectileSpeed * Time.deltaTime)));
                break;
            case "Angle down":
                melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionAngleDown);
                Fire(new Vector3(0, transform.eulerAngles.y, -45), new Vector2(velocityX, -(porjectileSpeed * Time.deltaTime)));
                break;
            case "Fire1":
                melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionForward);
                Fire(new Vector3(0f, transform.eulerAngles.y, 0f), new Vector2(velocityX, 0f));
                break;
        }
    }
    private void Jump()
    {
        if(doJump)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody2D.velocity += jumpVelocityToAdd;
            doJump = false;
        }
    }

    private void DoubleJump()
    {
        myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, 0f);
        Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
        myRigidbody2D.velocity += jumpVelocityToAdd;
    }

    private void Crouch()
    {
        if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) && verticalAxis == -1)
        {
            crouchCollider.enabled = true;
            myBodyColider2D.enabled = false;
            isCrouched = true;
            myAnimator.SetBool("Crouch", isCrouched);
            myAnimator.SetBool("Runing", !isCrouched);
        }
        else if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) && (verticalAxis > 0) &&
            !headCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && isCrouched)
        {
            crouchCollider.enabled = false;
            myBodyColider2D.enabled = true;
            isCrouched = false;
            myAnimator.SetBool("Crouch", isCrouched);
            myAnimator.SetBool("Crouch Moving", isCrouched);
        }
    }

    private void DoDash()
    {
        if (doDash)
        {
            myRigidbody2D.AddForce(new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * 150f, 0f), ForceMode2D.Impulse);
            dashEffect.SetActive(true);
            dashEffect.GetComponent<ParticleSystem>().Play();
            doDash = false;
        }
    }

    void Fire(Vector3 rotation, Vector2 velocity)
    {
        GameObject projectile = Instantiate(projectilePefab, melleAttack.transform.position, Quaternion.Euler(rotation)) as GameObject;
        projectile.GetComponent<Rigidbody2D>().velocity = velocity;
        projectile.GetComponent<FireBall>().MantainVelocity(velocity);
        if (DBManager.isInvader)
        {
            projectile.layer = 16;
            projectile.GetComponent<FireBall>().ProcessFieredShoot(melleAttack.transform.position, rotation);
        }
        else
        {
            projectile.GetComponent<FireBall>().ProcessFieredShoot(melleAttack.transform.position, rotation, true);
            projectile.GetComponent<SpriteRenderer>().color = new Color32(14, 255, 93, 255);
        }
    }

    Vector2 ChangeShottingPosition(Vector2 position)
    {
        Vector2 newest = new Vector2(transform.position.x, transform.position.y);
        Vector2 testPos = new Vector2(position.x * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180), position.y);
        position = (newest + testPos);
        return position;
    }

    void Hurt()
    {
        //print("my invader health: " + lastHealth + ", heatlh recived"+ currentHealth);
        if (lastHealth > currentHealth)
        {
            ChangeColor();
            myRigidbody2D.velocity = new Vector2(-Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (5f), 17f);
            waitForRecover = recoverCoolDown;
            lastHealth = currentHealth;
            healthBar.transform.localScale =
                new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, currentHealth / 500f, 75f * Time.deltaTime), 1, 1);
        }

        if(lastHealth < currentHealth)
        {
            lastHealth = currentHealth;
            healthBar.transform.localScale =
                new Vector3(Mathf.Lerp(healthBar.transform.localScale.x, currentHealth / 500f, 75f * Time.deltaTime), 1, 1);
        }

        if (currentHealth <= 0)
        {
            myAnimator.SetTrigger("Dying");
            healthBar.transform.localScale = new Vector3(0, 1, 1);

            if (!doDeathKik)
            {
                myRigidbody2D.velocity = new Vector2(-Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (10f), 25f);
                doDeathKik = true;
            }
           
            Destroy(gameObject, 1.5f);
        }
        if (healthBar.transform.localScale.x > 1)
        {
            healthBar.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ClimingState()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climing")))
        {
            myAnimator.SetBool("Climing", false);
            myRigidbody2D.gravityScale = gravatiySatetAtStart;
            return;
        }

        if (invaderInputs == "jump" || invaderInputs == "doubleJump")
        {
            jumpInStaires = true;
        }
        Vector2 playerVelocity = new Vector2(myRigidbody2D.velocity.x, verticalAxis * runSpeed);
        myRigidbody2D.velocity = playerVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody2D.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climing", playerHasVerticalSpeed);

        myRigidbody2D.gravityScale = 0f;
    }

    private void JumpInStairs()
    {
        if (jumpInStaires)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody2D.velocity += jumpVelocityToAdd;
            if (myRigidbody2D.velocity.y > jumpSpeed)
            {
                myRigidbody2D.velocity = jumpVelocityToAdd;
            }
            jumpInStaires = false;
        }
    }
    IEnumerator ChangeColor()
    {
        var myColor = GetComponent<SpriteRenderer>();
        if (!DBManager.isInvader)
        {
            myColor.color = new Color32(255, 0, 0, 80);
        }
        else
        {
            myColor.color = new Color32(255, 255, 255, 80);
        }
        yield return new WaitForSeconds(recoverCoolDown);
        if (!DBManager.isInvader)
        {
            myColor.color = new Color(255, 0, 0);
        }
        else
        {
            myColor.color = new Color(255, 255, 255);
        }
    }
}