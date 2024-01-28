using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour
{
    [SerializeField] GameObject flyingEnemy;
    [SerializeField] float maxHealth = 1500;
    float currentHealth;
    [SerializeField] float dropPercentage = 100f;
    [SerializeField] GameObject drop;
    bool hasDroped;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    Animator animator;
    private void Awake()
    {
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    IEnumerator Start()
    {
        while (true)
        {
            var existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (existingEnemies.Length <= 4)
            {
                print("lengt: " + existingEnemies.Length);
                yield return new WaitForSeconds(3f);
                Instantiate(flyingEnemy, transform.position, Quaternion.identity);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!enabled) return;
        DamageDealer damegeDealer = other.gameObject.GetComponent<DamageDealer>();

        if (!damegeDealer) { return; }
        ProcesssHit(damegeDealer);
    }

    private void ProcesssHit(DamageDealer damegeDealer)
    {
        currentHealth -= damegeDealer.Damage;
        StartCoroutine(SFX.ChangeColor(GetComponent<SpriteRenderer>()));
        gameScessionObj.GetComponent<AudioSource>().PlayOneShot(SFX.HurtAudioClips[Random.Range(0, SFX.HurtAudioClips.Length)],
         SFX.HurtVolume * SFX.VolumeMultiplier(transform.position));

        animator.SetFloat("Health", currentHealth / maxHealth);

        if (currentHealth <= 0)
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
