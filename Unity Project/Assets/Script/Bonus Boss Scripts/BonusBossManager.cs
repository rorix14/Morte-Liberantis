using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class BonusBossManager : MonoBehaviour
{
    [SerializeField] List<BonusBossPath> BossPaths;
    [SerializeField] GameObject missilePrfab;
    [SerializeField] List<Transform> fireDown;
    [SerializeField] List<Transform> fireLeft;
    [SerializeField] List<Transform> fireRight;
    [SerializeField] GameObject backGround;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;
    Action<float>[] fireFunctionsSage1;
    Action<float>[] fireFunctionsSage2;
    Action<float>[] fireFunctionsSage3;
    MinionSpawner minionSpawner;
    MoveBonusBoss moveBonusBoss;
    BossHealth bossHealth;
    SpriteRenderer imageColor;
    GameObject gameScession;
    bool doStage;
    float firstCooldown;
    float secundCooldown;
    float bomSpeed = 750f;
    float backgroundfade = 1;
    private void Awake()
    {
        if (gameScession == null)
        {
            gameScession = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }
    void Start()
    {
        moveBonusBoss = GetComponent<MoveBonusBoss>();
        bossHealth = FindObjectOfType<BossHealth>().GetComponent<BossHealth>();
        imageColor = GetComponent<SpriteRenderer>();
        minionSpawner = GetComponent<MinionSpawner>();

        fireFunctionsSage1 = new Action<float>[] { FireDown, FirePaternMultipleDirections, FireDown };
        fireFunctionsSage2 = new Action<float>[] { FireDown, (f) => { }, FireDown, (f) => { }, FireDown };
        fireFunctionsSage3 = new Action<float>[] { FireDown, (f) => { }, FirePaternMultipleDirections, (f) => { }, FireDown };

        moveBonusBoss.SetPathConfig(BossPaths[0], fireFunctionsSage1);
    }

    void Update()
    {
        if (bossHealth.BossHP <= 0) return;
        ManageBossSate();

        StageTwoEffects();
    }

    void ManageBossSate()
    {
        switch (bossHealth.BossHP / bossHealth.MaxHealth)
        {
            case 0.8f:
                if (!doStage)
                {
                    moveBonusBoss.SetPathConfig(BossPaths[1], fireFunctionsSage2);
                    doStage = true;
                }
                break;
            case 0.6f:
                if (doStage)
                {
                    moveBonusBoss.SetPathConfig(BossPaths[0], fireFunctionsSage1, 1.5f, 0.7f);
                    doStage = false;
                }
                break;
            case 0.4f:
                if (!doStage)
                {
                    moveBonusBoss.SetPathConfig(BossPaths[1], fireFunctionsSage2, 2f, 0.7f);
                    doStage = true;
                }
                break;
            case 0.3f:
                if (doStage)
                {
                    moveBonusBoss.SetPathConfig(BossPaths[2], fireFunctionsSage3, 3f, 0.5f);
                    doStage = false;
                }
                break;
        }
    }

    void FireDown(float reduceCooldown)
    {
        var vel = new Vector2(bomSpeed * Mathf.Cos(270 * Mathf.PI / 180), bomSpeed * Mathf.Sin(270 * Mathf.PI / 180));

        foreach (var transform in fireDown)
        {
            firstCooldown -= Time.deltaTime;
            if (firstCooldown <= 0)
            {
                Fire(transform.position, new Vector3(0, 0, 0), vel);
                firstCooldown = 1.5f * reduceCooldown;
            }
        }
    }

    void FirePaternMultipleDirections(float reduceCooldown)
    {
        secundCooldown -= Time.deltaTime;
        if (secundCooldown <= 0)
        {
            FireByPoints(fireDown, 0);
            FireByPoints(fireLeft, -20);
            FireByPoints(fireRight, +20);
            secundCooldown = 1.5f * reduceCooldown;
        }
    }

    void FireByPoints(List<Transform> fiaringPositions, float rotToAdd)
    {
        var rot = 270f;

        foreach (var transform in fiaringPositions)
        {
            rot += rotToAdd;
            Vector2 vel = new Vector2(bomSpeed * Mathf.Cos(rot * Mathf.PI / 180), bomSpeed * Mathf.Sin(rot * Mathf.PI / 180));

            Fire(transform.position, new Vector3(0, 0, rot - 270), vel);
        }
    }

    void Fire(Vector3 position, Vector3 rotaion, Vector2 velocity)
    {
        GameObject projectile;
        projectile = Instantiate(missilePrfab, position, Quaternion.Euler(rotaion));
        projectile.GetComponent<Rigidbody2D>().velocity = velocity * Time.deltaTime;
        projectile.GetComponent<Missile>().ProcessFieredShoot(position, rotaion);
        gameScession.GetComponent<AudioSource>().PlayOneShot(SFX.BossShootAudioClips[UnityEngine.Random.Range(0, SFX.BossShootAudioClips.Length)], SFX.BoosShootVolume);
    }

    void StageTwoEffects()
    {
        if (bossHealth.BossHP / bossHealth.MaxHealth <= 0.6f)
        {
            minionSpawner.enabled = true;
            bossHealth.GetComponent<AudioSource>().enabled = true;

            backgroundfade -= (0.4f * Time.deltaTime);
            if (backgroundfade >= 0)
            {
                backGround.GetComponent<Tilemap>().color = new Color32(255, 255, 255, (byte)((backgroundfade) * 255));
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        DamageDealer damegeDealer = other.gameObject.GetComponent<DamageDealer>();

        if (!damegeDealer) { return; }
        StartCoroutine(ChangeColor());
        bossHealth.ProcessBossHealth(-damegeDealer.Damage);
    }

    IEnumerator ChangeColor()
    {
        imageColor.color = new Color32(217, 15, 15, 255);
        yield return new WaitForSeconds(0.2f);
        imageColor.color = new Color(255, 255, 255);
    }
}
