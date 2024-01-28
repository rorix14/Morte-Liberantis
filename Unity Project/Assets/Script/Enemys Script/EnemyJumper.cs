using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumper : MonoBehaviour
{
    [SerializeField] float junpeDirectionX = 7f;
    [SerializeField] float junpeDirectionY = 25f;
    [SerializeField] float health = 500;
    [SerializeField] float dropPercentage = 100f;
    [SerializeField] GameObject drop;
    BoxCollider2D boxCollider2D;
    Rigidbody2D rg;
    bool doJump;
    Vector2 eulerAngles;
    float nextJump;
    float coolDown = 1;
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
        boxCollider2D = GetComponent<BoxCollider2D>();
        rg = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boxCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            doJump = true;
        }
    }
    private void FixedUpdate()
    {
        Jump();
    }

    void Jump()
    {
        if (boxCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")) && Time.time > nextJump)
        {
            if (doJump)
            {
                rg.AddForce(new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * junpeDirectionX, junpeDirectionY), ForceMode2D.Impulse);
                gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.JumpAudioClips[Random.Range(0, SFX.JumpAudioClips.Length)],
      SFX.JumpVolume * SFX.VolumeMultiplier(transform.position));
                nextJump = coolDown + Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

        DamageDealer damegeDealer = collision.gameObject.GetComponent<DamageDealer>();

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
