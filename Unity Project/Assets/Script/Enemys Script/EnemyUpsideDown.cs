using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUpsideDown : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 500;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float dropPercentage = 100f;
    [SerializeField] GameObject drop;
    [Header("Projectile")]
    [SerializeField] GameObject projectilePefab;
    [SerializeField] float projectileSpeed = -700f;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;

    Rigidbody2D myRigidBody2D;
    Vector2 eulerAngles;
    BoxCollider2D enemyFeet;
    bool hasDroped;
    Animator myAnimator;
    float timer;
    string state = "wallk";
    private void Awake()
    {
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        enemyFeet = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();

        StartCoroutine(enumerator());
    }

    // Update is called once per frame
    void Update()
    {
        ManageState();
    }

    IEnumerator enumerator()
    {
        while (true)
        {
            state = "wallk";

            yield return new WaitForSeconds(6f);

            timer = Time.time;
            state = "shoot";

            yield return new WaitForSeconds(4f);
        }
    }
    void ManageState()
    {
        switch (state)
        {
            case "shoot":
                Shoot();
                myAnimator.SetBool("stop", true);
                myRigidBody2D.velocity = new Vector2(0, 0);
                break;
            case "wallk":
                myAnimator.SetBool("stop", false);

                if (!enemyFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

                if (isFacingRight())
                {
                    myRigidBody2D.velocity = new Vector2(moveSpeed, myRigidBody2D.velocity.y);
                }
                else
                {
                    myRigidBody2D.velocity = new Vector2(-moveSpeed, myRigidBody2D.velocity.y);
                }
                break;
        }
    }
    void Shoot()
    {
        if (Time.time - timer > 1)
        {
            GameObject projectile = Instantiate(projectilePefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 270))) as GameObject;
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, Time.deltaTime * projectileSpeed);
            projectile.GetComponent<FireBall>().ProcessFieredShoot(transform.position, transform.eulerAngles);
            //gameScessionObj.GetComponent<AudioSource>().PlayOneShot(shootAudioClips[Random.Range(0, shootAudioClips.Length)], volume * VolumeMultiplier());
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.ShootAudioClips[Random.Range(0, SFX.ShootAudioClips.Length)],
                SFX.ShootVolume * SFX.VolumeMultiplier(transform.position));
            timer = Time.time;
        }
    }
    private bool isFacingRight()
    {
        return transform.eulerAngles.y == 0;
        // return transform.localScale.x > 0;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled) return;
        //transform.localScale = new Vector2(-Mathf.Sign(myRigidBody2D.velocity.x), 1f);
        if (collision.tag != "Forground")
        {
            if (transform.eulerAngles.y == 0f)
            {
                eulerAngles.y = 180f;
            }
            else if ((transform.eulerAngles.y == 180f))
            {
                eulerAngles.y = 0;
            }
            transform.eulerAngles = eulerAngles;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (!enabled) return;
        DamageDealer damegeDealer = other.gameObject.GetComponent<DamageDealer>();

        if (!damegeDealer) { return; }
        ProcesssHit(damegeDealer);
    }
    private void ProcesssHit(DamageDealer damegeDealer)
    {
        health -= damegeDealer.Damage;
        StartCoroutine(SFX.ChangeColor(GetComponent<SpriteRenderer>()));
        gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.HurtAudioClips[Random.Range(0, SFX.HurtAudioClips.Length)],
         SFX.HurtVolume * SFX.VolumeMultiplier(transform.position));
        // damegeDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        DropItemOnDeath();
        Instantiate(SFX.DestroyEffect, transform.position, Quaternion.identity);
        gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.DestroyedAudioClips[Random.Range(0, SFX.DestroyedAudioClips.Length)],
        SFX.DestroyedVolume * SFX.VolumeMultiplier(transform.position));
        gameScessionObj.GetComponent<GameScession>().RemoveEnemy(gameObject);
        Destroy(gameObject);
    }

    private void DropItemOnDeath()
    {
        if (dropPercentage >= Random.Range(0f, 100f))
        {
            if (!hasDroped)
            {
                Instantiate(drop, transform.position, Quaternion.identity);
                hasDroped = true;
            }
        }
    }
}
