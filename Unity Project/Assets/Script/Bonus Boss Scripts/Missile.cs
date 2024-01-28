using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float health = 50;
    [SerializeField] GameObject explosionLight;
    [SerializeField] GameObject fieredEffect;
    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    float explosionRadius;
    Animator myAnimator;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    bool hasPlayedSound;

    void Start()
    {
        gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        myAnimator = GetComponent<Animator>();
        myAnimator.SetTrigger("Fall");
    }

    void Update()
    {
        ExplosionRadiusOverTime();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        circleCollider.enabled = true;
    }

    void ExplosionRadiusOverTime()
    {
        if (!circleCollider.enabled) { return; }

        rb.velocity = new Vector2(0, 0);
        myAnimator.SetTrigger("Exploded");

        if (explosionLight != null)
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
        gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.HurtAudioClips[Random.Range(0, SFX.HurtAudioClips.Length)], SFX.HurtVolume);

        if (health <= 0)
        {
            circleCollider.enabled = true;
        }
    }

    public void ProcessFieredShoot(Vector3 position, Vector3 rotation, bool chageColor = false)
    {
        GameObject projectile = Instantiate(fieredEffect, position, Quaternion.Euler(rotation)) as GameObject;
        if (chageColor)
        {
            projectile.GetComponent<SpriteRenderer>().color = new Color32(14, 255, 93, 255);
        }
        Destroy(projectile, 0.2f);
    }
}
