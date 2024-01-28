using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] BoxCollider2D coneOfVision;
    [SerializeField] float health = 150;
    [SerializeField] GameObject explosionLight;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    Player player;
    Vector3 onSeenPlayerPosition;
    bool hasCollided;
    float explosionRadius;
    Animator myAnimator;
    [SerializeField] GameObject drop;
    [SerializeField] float dropPercentage = 100f;
    bool hasDroped;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    bool hasPlayedSound;
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
        player = FindObjectOfType<Player>();
        circleCollider = GetComponent<CircleCollider2D>();
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        MoveToPlayerPosition();
        ExplosionRadiusOverTime();
    }

    void MoveToPlayerPosition()
    {
        if (coneOfVision.enabled)
        {
            if (coneOfVision.IsTouchingLayers(LayerMask.GetMask("Player", "PlayerInvader")))
            {
                onSeenPlayerPosition = player.transform.position;
                coneOfVision.enabled = false;
            }
        }
        else
        {
            if (hasCollided) { return; }
            rb.velocity = new Vector2(onSeenPlayerPosition.x - transform.position.x, Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180) * -10f);
            myAnimator.SetTrigger("Fall");
            // transform.position = Vector2.MoveTowards(transform.position, onSeenPlayerPosition, 20 * Time.deltaTime);
            //rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasCollided = true;
        circleCollider.enabled = true;
    }

    void ExplosionRadiusOverTime()
    {
        if (!circleCollider.enabled) { return; }

        rb.velocity = new Vector2(0, 0);
        myAnimator.SetTrigger("Exploded");

        if(explosionLight != null)
        {
            explosionLight.SetActive(true);
        }

        if (!hasPlayedSound)
        {
            gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.ExplosionAudioClip, SFX.ExplosionVolume);
            hasPlayedSound = true;
        }

        if (explosionRadius <= 1f)
        {
            explosionRadius += 5f * Time.deltaTime;
            circleCollider.radius = explosionRadius;
        }

        Destroy(explosionLight, 0.4f);
        Destroy(gameObject, 0.7f);
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
        rb.velocity = new Vector2(0, -10);
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
        circleCollider.enabled = true;
        gameScessionObj.GetComponent<GameScession>().RemoveEnemy(gameObject);
        DropItemOnDeath();
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
