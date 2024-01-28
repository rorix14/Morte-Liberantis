using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 500;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float dropPercentage = 100f;
    [SerializeField] GameObject drop;
    Rigidbody2D myRigidBody2D;
    Vector2 eulerAngles;
    BoxCollider2D enemyFeet;
    [SerializeField] GameObject enemy;
    bool hasDroped;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    // Start is called before the first frame update
    private void Awake()
    {
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        // enemyFeet = enemy.GetComponent<BoxCollider2D>();
        enemyFeet = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (isFacingRight())
        {
            myRigidBody2D.velocity = new Vector2(moveSpeed, myRigidBody2D.velocity.y);
        }
        else
        {
            myRigidBody2D.velocity = new Vector2(-moveSpeed, myRigidBody2D.velocity.y);
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
