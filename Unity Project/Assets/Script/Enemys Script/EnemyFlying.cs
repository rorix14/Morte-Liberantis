using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] GameObject pathConfig;
    [SerializeField] float health = 500;
    [SerializeField] float dropPercentage = 100f;
    [SerializeField] GameObject drop;
    bool hasDroped;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    Transform player;
    private void Awake()
    {
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Start()
    {
        pathConfig.GetComponent<AIDestinationSetter>().target = player;
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
        gameScessionObj.GetComponent<GameScession>().RemoveEnemy(pathConfig);
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
    private void OnDestroy()
    {
        Destroy(pathConfig);
    }
}
