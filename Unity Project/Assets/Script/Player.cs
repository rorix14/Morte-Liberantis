using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Player configurations
    [Header("Player")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climSpeed = 1f;
    [SerializeField] float dashForce = 60f;
    //[SerializeField] float maxHealth = 600f;
    //[SerializeField] float playerHealth;
    [SerializeField] GameObject dashEffect;
    [SerializeField] GameObject healthBar;
    Vector2 deathKick; //= new Vector2(25f, 25f);

    // [SerializeField] [Range(0, 1)] float gameSpeed = 1f;

    // test dash///////
    [SerializeField] float coolDownDash = 2f;
    float nextDash = 0;

    [Header("Coliders")]
    [SerializeField] CapsuleCollider2D crouchCollider;
    [SerializeField] BoxCollider2D headCollider;

    // Projectile config
    [Header("Projectile")]
    [SerializeField] GameObject projectilePefab;
    [SerializeField] float porjectileSpeed = 1700f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    [SerializeField] GameObject fieredEffect;
    [SerializeField] AudioClip[] shootAudioClips;
    [SerializeField] [Range(0, 1)] float shootVolume = 0.75f;
    float nextShoot = 0;

    [Header("Melle Atack")]
    [SerializeField] GameObject melleAttack;

    [Header("SFX")]
    [SerializeField] AudioClip[] hurtAudioClips;
    [SerializeField] [Range(0, 1)] float hurtVolume = 0.133f;
    [SerializeField] AudioClip[] jumpAudioClips;
    [SerializeField] [Range(0, 1)] float jumpVolume = 0.133f;
    [SerializeField] AudioClip[] doubleJumpAudioClips;
    [SerializeField] [Range(0, 1)] float doubleJumpVolume = 0.133f;
    [SerializeField] AudioClip[] dashAudioClips;
    [SerializeField] [Range(0, 1)] float dashVolume = 0.133f;
    [SerializeField] AudioClip deathClip;
    [SerializeField] [Range(0, 1)] float deathVolume = 0.133f;

    // States
    bool isAlive = true;
    string shootingState;
    bool canDoubleJump = false;
    bool isCrouched = false;
    bool doDash = false;
    bool jumpInStairs = false;

    // Cached components references
    Rigidbody2D myRigidbody2D;
    Animator myAnimator;
    CapsuleCollider2D myBodyColider2D;
    BoxCollider2D myFeet;
    GameScession gameScession;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    float gravatiySatetAtStart;
    //EnemyMovement enemy;
    Coroutine friringCoroutine;
    Vector3 eulerAngles;
    //Transform bar;
    float waitForRecover = 0f;
    float recoverCoolDown = 0.5f;
    float attackDoration = 0f;
    float controlTrow;
    float verticalAxis;
    string[] playerInputs;
    [SerializeField] GameObject invaderSpawnPoint;

    /*public float MaxHealth
    {
        get { return maxHealth; }
    }
    public float PlayerHealth
    {
        get { return playerHealth; }
        set { playerHealth = value; }
    }*/
    public float AttackDoration
    {
        get { return attackDoration; }
        set { attackDoration = value; }
    }
    public float NextDash
    {
        get { return nextDash; }
    }
    public float CoolDownDash
    {
        get { return coolDownDash; }
    }
    public float ControlTrow
    {
        get { return controlTrow; }
    }
    public float VerticalAxis
    {
        get { return verticalAxis; }
    }
    public string[] PlayerInputs
    {
        get { return playerInputs; }
        set { playerInputs = value; }
    }
    private void Awake()
    {
        /*int numPlayer = FindObjectsOfType<Player>().Length;

        if (numPlayer > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }*/
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }
    void Start()
    {
        if (DBManager.isInvader)
        {
            gameObject.layer = 20;
            transform.position = invaderSpawnPoint.transform.position;
            GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        }
        //playerHealth = maxHealth;
        friringCoroutine = StartCoroutine(Null());
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyColider2D = GetComponent<CapsuleCollider2D>();
        myFeet = GetComponent<BoxCollider2D>();
        gravatiySatetAtStart = myRigidbody2D.gravityScale;
        //enemy = FindObjectOfType<EnemyMovement>();
        gameScession = FindObjectOfType<GameScession>();
    }

    void Update()
    {
        //Time.timeScale = gameSpeed;
        if (!isAlive) { return; }

        if (waitForRecover > 0)
        {
            try
            {
                StopCoroutine(friringCoroutine);
            }
            catch (Exception e)
            {
                Debug.Log("Exception caught. " + e);
            }
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

        Run();
        ClimingState();

        if (Input.GetButton("Dash") && DBManager.DashPWUp && Time.time > nextDash) doDash = true;

        Crouch();
        melleAttack.GetComponent<MelleAttack>().DoMelleAttack(transform.eulerAngles);

        if (DBManager.DoubleJumpPWUp && !isCrouched) { DoubleJump(); } else if (!DBManager.DoubleJumpPWUp && !isCrouched) { Jump(); }
        try
        {
            if (gameScessionObj.GetComponent<GameScession>().getAmmo > 0 && !gameScessionObj.GetComponent<GameScession>().getIsPaused) { SeeInputToFire(); } else { StopCoroutine(friringCoroutine); }
            // if (DBManager.AmmoAmought > 0 && !gameScession.getIsPaused) { SeeInputToFire(); } else { StopCoroutine(friringCoroutine) }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        Death();
    }

    private void FixedUpdate()
    {
        DoDash();
        JumpInStairs();
    }

    private void Run()
    {
        //CrossPlatformInputManager
        controlTrow = Input.GetAxis("Horizontal"); // value -1 to +1
        //print("test player movemnt: " + controlTrow);
        Vector2 playerVelocity = new Vector2(controlTrow * runSpeed, myRigidbody2D.velocity.y);
        if (isCrouched)
        {
            myRigidbody2D.velocity = new Vector2((controlTrow * runSpeed) / 2, myRigidbody2D.velocity.y);
        }
        else myRigidbody2D.velocity = playerVelocity;

        if (controlTrow < 0)
        {
            eulerAngles.y = 180f;
        }
        else if (controlTrow > 0)
        {
            eulerAngles.y = 0;
        }
        else if (controlTrow == 0)
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
    private void ClimingState()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climing")))
        {
            myAnimator.SetBool("Climing", false);
            myRigidbody2D.gravityScale = gravatiySatetAtStart;
            return;
        }

        //CrossPlatformInputManager
        float controlTrow = Input.GetAxis("Vertical"); // value -1 to +1
        Vector2 playerVelocity = new Vector2(myRigidbody2D.velocity.x, controlTrow * runSpeed);
        myRigidbody2D.velocity = playerVelocity;

        if (controlTrow == 0 && !headCollider.IsTouchingLayers(LayerMask.GetMask("Climing")))
        {
            if (Input.GetButtonDown("Jump"))
            {
                playerInputs = new string[1] { "jump" };
                gameScessionObj.GetComponent<AudioSource>().PlayOneShot(jumpAudioClips[UnityEngine.Random.Range(0, jumpAudioClips.Length)], jumpVolume);
                jumpInStairs = true;
            }
        }

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody2D.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climing", playerHasVerticalSpeed);

        myRigidbody2D.gravityScale = 0f;
    }

    private void JumpInStairs()
    {
        if (jumpInStairs)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody2D.velocity += jumpVelocityToAdd;
            canDoubleJump = true;
            if (myRigidbody2D.velocity.y > jumpSpeed)
            {
                myRigidbody2D.velocity = jumpVelocityToAdd;
            }
            jumpInStairs = false;
        }
    }

    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        //CrossPlatformInputManager
        if (Input.GetButtonDown("Jump"))
        {
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(jumpAudioClips[UnityEngine.Random.Range(0, jumpAudioClips.Length)], jumpVolume);
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody2D.velocity += jumpVelocityToAdd;
            playerInputs = new string[1] { "jump" };
        }
    }
    private void Death()
    {
        if (myBodyColider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard", "RisingWater", "EnemyPatrolShoot", "InvaderProjectiles")) ||
            crouchCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard", "RisingWater", "EnemyPatrolShoot", "InvaderProjectiles")) 
            || (DBManager.isInvader && myBodyColider2D.IsTouchingLayers(LayerMask.GetMask("PlayerProjectiles"))) || (DBManager.isInvader && crouchCollider.IsTouchingLayers(LayerMask.GetMask("PlayerProjectiles"))))
        {
            myRigidbody2D.velocity = new Vector2(-Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (5f), 17f);
            //myRigidbody2D.AddForce(new Vector2(-Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (5f), 17f), ForceMode2D.Impulse);

            gameScessionObj.GetComponent<GameScession>().ProcessPlayerHealth(-50);
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(hurtAudioClips[UnityEngine.Random.Range(0, hurtAudioClips.Length)], hurtVolume);
            StartCoroutine(ChangeColor());
            waitForRecover = recoverCoolDown;

            if (myBodyColider2D.IsTouchingLayers(LayerMask.GetMask("Hazard")) || crouchCollider.IsTouchingLayers(LayerMask.GetMask("Hazard")))
            {
                gameScessionObj.GetComponent<GameScession>().ProcessPlayerHealth(-gameScessionObj.GetComponent<GameScession>().MaxHealth);
            }

            if (gameScessionObj.GetComponent<GameScession>().PlayerHealth <= 0)
            {
                myAnimator.SetTrigger("Dying");
                deathKick = new Vector2(-Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (10f), 25f);
                myRigidbody2D.velocity = deathKick;
                gameScessionObj.GetComponent<AudioSource>().PlayOneShot(deathClip, deathVolume);
                isAlive = false; 
            }
            //healthBar.transform.Find("Bar").Find("Bar sprite").localScale = new Vector3(playerHealth/maxHealth, 1, 1);
            //FindObjectOfType<GameScession>().ProcessPlayerDeath();
        }
    }
    private void Crouch()
    {
        verticalAxis = Input.GetAxis("Vertical");

        if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) && (Input.GetAxis("Vertical") == -1))
        {
            crouchCollider.enabled = true;
            myBodyColider2D.enabled = false;
            isCrouched = true;
            myAnimator.SetBool("Crouch", isCrouched);
            myAnimator.SetBool("Runing", !isCrouched);
        }
        else if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) && (Input.GetAxis("Vertical") > 0) &&
            !headCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && isCrouched)
        {
            crouchCollider.enabled = false;
            myBodyColider2D.enabled = true;
            isCrouched = false;
            myAnimator.SetBool("Crouch", isCrouched);
            myAnimator.SetBool("Crouch Moving", isCrouched);
        }
    }
    private void SeeInputToFire()
    {
        float rotationX;
        float velocityX = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * (porjectileSpeed * Time.deltaTime);

        if (Input.GetButton("Look up") || Input.GetAxis("Look up") >= 0.7f) shootingState = "Look up";

        else if (Input.GetButton("Angle up")) shootingState = "Angle up";

        else if (Input.GetButton("Angle down")) shootingState = "Angle down";

        else if (Input.GetButtonDown("Fire1") || Input.GetAxis("Fire With controler") == 1) shootingState = "Fire1";

        switch (shootingState)
        {
            case "Look up":
                // melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionUp);
                FireUpOrOnAngles(shootingState, new Vector3(0, transform.eulerAngles.y, 90), new Vector2(0f, (porjectileSpeed * Time.deltaTime)),
                    melleAttack.GetComponent<MelleAttack>().PositionUp);
                break;
            case "Angle up":
                // melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionAngleUp); shootingState = "Angle up";
                rotationX = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) == 1 ? 45 : 135;

                FireUpOrOnAngles(shootingState, new Vector3(0, transform.eulerAngles.y, 45), new Vector2(velocityX, (porjectileSpeed * Time.deltaTime)),
                   melleAttack.GetComponent<MelleAttack>().PositionAngleUp);
                break;
            case "Angle down":
                //melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionAngleDown);
                rotationX = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) == 1 ? 315 : 225;

                FireUpOrOnAngles(shootingState, new Vector3(0, transform.eulerAngles.y, -45), new Vector2(velocityX, -(porjectileSpeed * Time.deltaTime)),
                    melleAttack.GetComponent<MelleAttack>().PositionAngleDown);
                break;
            case "Fire1":
                //melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionForward);
                Fire(new Vector3(0f, transform.eulerAngles.y, 0f), new Vector2(velocityX, 0f));
                break;
        }
    }
    private void Fire(Vector3 rotation, Vector2 velocity)
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetAxis("Fire With controler") == 1) && Time.time > nextShoot)
        {
            melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionForward);
            friringCoroutine = StartCoroutine(FireContinuouslyUpwarsAndOnAngle(rotation, velocity));
            // ProcessFieredShoot();
            nextShoot = Time.time + projectileFiringPeriod;
        }
        if (Input.GetButtonUp("Fire1") || Input.GetAxis("Fire With controler") == 1)
        {
            try
            {
                StopCoroutine(friringCoroutine);
            }
            catch (Exception e)
            {
                Debug.Log("Exception caught. " + e);
            }
        }
    }
    private void FireUpOrOnAngles(string angle, Vector3 rotation, Vector2 velocity, Vector2 shootingPlacePosition)
    {
        if (((Input.GetButtonDown("Fire1") || Input.GetAxis("Fire With controler") == 1)
            && (Input.GetButton(angle) || Input.GetAxis(angle) >= 0.7f)) && Time.time > nextShoot)
        {
            melleAttack.transform.position = ChangeShottingPosition(shootingPlacePosition);
            friringCoroutine = StartCoroutine(FireContinuouslyUpwarsAndOnAngle(rotation, velocity));
            // ProcessFieredShoot();
            nextShoot = Time.time + projectileFiringPeriod;
        }

        if ((Input.GetButtonUp("Fire1") || Input.GetAxis("Fire With controler") == 1) || Input.GetButtonUp(angle))
        {
            melleAttack.transform.position = ChangeShottingPosition(melleAttack.GetComponent<MelleAttack>().PositionForward);
            try
            {
                StopCoroutine(friringCoroutine);
            }
            catch (Exception e)
            {
                Debug.Log("Exception caught. " + e);
            }
        }
    }
    IEnumerator FireContinuouslyUpwarsAndOnAngle(Vector3 rotation, Vector2 velocity)
    {
        while (true)
        {
            playerInputs = new string[1] { shootingState };
            GameObject projectile = Instantiate(projectilePefab, melleAttack.transform.position, Quaternion.Euler(rotation)) as GameObject;
            projectile.GetComponent<Rigidbody2D>().velocity = velocity;
            projectile.GetComponent<FireBall>().MantainVelocity(velocity);
            // projectile.transform.localScale = scale;
            gameScessionObj.GetComponent<GameScession>().getAmmo--;
            //DBManager.AmmoAmought = gameScessionObj.GetComponent<GameScession>().getAmmo;
            gameScessionObj.GetComponent<GameScession>().updateScore();
            if (DBManager.isInvader)
            {
                projectile.GetComponent<FireBall>().ProcessFieredShoot(melleAttack.transform.position, rotation, true);
                projectile.GetComponent<SpriteRenderer>().color = new Color32(14, 255, 93, 255);
            }
            else
            {
                projectile.GetComponent<FireBall>().ProcessFieredShoot(melleAttack.transform.position, rotation);

            }
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(shootAudioClips[UnityEngine.Random.Range(0, shootAudioClips.Length)], shootVolume);
            if (transform.eulerAngles.y != projectile.transform.eulerAngles.y) break;
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }
    IEnumerator Null()
    {
        yield return null;
    }
    public void DoubleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                gameScessionObj.GetComponent<AudioSource>().PlayOneShot(jumpAudioClips[UnityEngine.Random.Range(0, jumpAudioClips.Length)], jumpVolume);
                Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
                myRigidbody2D.velocity += jumpVelocityToAdd;
                canDoubleJump = true;
                playerInputs = new string[1] { "jump" };
            }
            else if (canDoubleJump == true)
            {
                gameScessionObj.GetComponent<AudioSource>().PlayOneShot(doubleJumpAudioClips[UnityEngine.Random.Range(0, doubleJumpAudioClips.Length)], doubleJumpVolume);
                myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, 0f);
                Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
                myRigidbody2D.velocity += jumpVelocityToAdd;
                canDoubleJump = false;
                playerInputs = new string[1] { "doubleJump" };
            }
        }
    }
    public void DoDash()
    {
        // if ( Time.time > nextDash)
        if (doDash)
        {
            playerInputs = new string[1] { "dash" };
            doDash = false;
            myRigidbody2D.AddForce(new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * 150f, 0f), ForceMode2D.Impulse);
            /*Vector2 jumpVelocityToAdd = new Vector2(transform.forward.z > 0 ? dashForce : -dashForce, 0f);
            myRigidbody2D.velocity += jumpVelocityToAdd;*/
            dashEffect.SetActive(true);
            dashEffect.GetComponent<ParticleSystem>().Play();
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(dashAudioClips[UnityEngine.Random.Range(0, dashAudioClips.Length)], dashVolume);
            nextDash = Time.time + coolDownDash;
        }
    }

    Vector2 ChangeShottingPosition(Vector2 position)
    {
        Vector2 newest = new Vector2(transform.position.x, transform.position.y);
        Vector2 testPos = new Vector2(position.x * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180), position.y);
        position = (newest + testPos);
        return position;
    }

    IEnumerator ChangeColor()
    {
        var myColor = GetComponent<SpriteRenderer>();
        if (DBManager.isInvader)
        {
            myColor.color = new Color32(255, 0, 0, 80);
        }
        else
        {
            myColor.color = new Color32(255, 255, 255, 80);
        }
        yield return new WaitForSeconds(recoverCoolDown);
        if (DBManager.isInvader)
        {
            myColor.color = new Color(255, 0, 0);
        }
        else
        {
            myColor.color = new Color(255, 255, 255);
        }
    }
    /*USED TO ISNTANCIATE FIRE EFFECT PREFAB WHEN SHOOT, REFACTURED TO FIREBALL SCRIPT 
    public void ProcessFieredShoot()
    {
        ///GameObject projectile = Instantiate(fieredEffect, transform.position, transform.rotation) as GameObject;
        GameObject projectile = Instantiate(fieredEffect, melleAttack.transform.position, transform.rotation) as GameObject;
        //projectile.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
        Destroy(projectile, 0.2f);
    }*/
    /* private void OnTriggerEnter2D(Collider2D other)
     {
         Debug.Log("fire");
         DamageDealer damegeDealer = other.gameObject.GetComponent<DamageDealer>();
         string obj = other.name;
         if (!damegeDealer) { return; }
         FindObjectOfType<GameScession>().ProcessPlayerDeath();
     }*/
    // FLIP SPRITE BASED ON LOCAL SCALE
    /* private void FlipSprite()
     {
         bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody2D.velocity.x) > Mathf.Epsilon; // if graeter than 0
         if (playerHasHorizontalSpeed)
         {
             transform.localScale = new Vector2(Mathf.Sign(myRigidbody2D.velocity.x), 1f);
         }
     }*/

    IEnumerator Test()
    {
        Debug.Log("TEST 1");
        yield return new WaitForSeconds(2.0f);
        myRigidbody2D.velocity = new Vector2(transform.localScale.x * dashForce, myRigidbody2D.velocity.y);
        Debug.Log("TEST 2");
    }
    float currCountdownValue;
    IEnumerator StartCountdown(float countdownValue = 10)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            myRigidbody2D.velocity = new Vector2(transform.localScale.x * dashForce, myRigidbody2D.velocity.y);

            Debug.Log("Countdown: " + currCountdownValue);
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
    }
}