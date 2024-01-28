using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZigZag : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float Xvel = -4f;
    [SerializeField] float Yvel = 1.5f;
    [SerializeField] float health = 500;
    [SerializeField] float dropPercentage = 100f;
    [SerializeField] GameObject drop;
    [SerializeField] GameObject seeTop;
    [SerializeField] GameObject seeBottom;
    [SerializeField] GameObject seeAhead;
    Rigidbody2D myRigidBody2D;
    bool hasDroped;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
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
    }

    void FixedUpdate()
    {
        GiveVelocity();
        ChangeVelocityAndDirction();
    }

    void GiveVelocity()
    {
        myRigidBody2D.velocity = new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * Xvel, Yvel);
    }
    void ChangeVelocityAndDirction()
    {
        if (seeTop.GetComponent<CircleCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Yvel = -Mathf.Abs(Yvel);
        }
        if (seeBottom.GetComponent<CircleCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Yvel = Mathf.Abs(Yvel);
        }
        if (seeAhead.GetComponent<CircleCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
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
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Instantiate(SFX.DestroyEffect, transform.position, Quaternion.identity);
        gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.DestroyedAudioClips[Random.Range(0, SFX.DestroyedAudioClips.Length)],
        SFX.DestroyedVolume * SFX.VolumeMultiplier(transform.position));
        DropItemOnDeath();
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
