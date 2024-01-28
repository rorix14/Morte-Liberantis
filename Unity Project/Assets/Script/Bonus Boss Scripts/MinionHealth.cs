using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHealth : MonoBehaviour
{
    [SerializeField] WaveConfig minionSetings;
    GameScession gameScession;
    [SerializeField] Transform[] fieringDownPositionOne;
    [SerializeField] Transform[] fieringDownPositionTwo;
    [SerializeField] Transform[] fieringDownPositionThree;
    Transform[][] fieringDownPositions;
    [Header("SFX")]
    [SerializeField] EnemySFX SFX;

    private void Start()
    {
        gameScession = FindObjectOfType<GameScession>().GetComponent<GameScession>();

        if (!minionSetings.MinionHealth.HasValue) minionSetings.SetToMaxHealth();

        if (minionSetings.MinionHealth <= 0 && minionSetings.MinionHealth.HasValue) Destroy(gameObject);

        fieringDownPositions = new Transform[][] { VerifyArryNulability(fieringDownPositionOne), VerifyArryNulability(fieringDownPositionTwo), VerifyArryNulability(fieringDownPositionThree) };

        StartCoroutine(minionSetings.FieringRutine(fieringDownPositions, SFX, gameScession));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;
        DamageDealer damegeDealer = other.gameObject.GetComponent<DamageDealer>();

        if (!damegeDealer) { return; }
        ProcessMinionHelth(damegeDealer.Damage);
    }

    void ProcessMinionHelth(float damageTaken)
    {
        minionSetings.MinionHealth -= damageTaken;
        StartCoroutine(SFX.ChangeColor(GetComponent<SpriteRenderer>()));

        gameScession.GetComponent<AudioSource>().PlayOneShot(SFX.HurtAudioClips[Random.Range(0, SFX.HurtAudioClips.Length)],
            SFX.HurtVolume * SFX.VolumeMultiplier(transform.position));

        if (minionSetings.MinionHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        var effect = Instantiate(SFX.DestroyEffect, transform.position, Quaternion.identity);

        gameScession.GetComponent<AudioSource>().PlayOneShot(SFX.DestroyedAudioClips[Random.Range(0, SFX.DestroyedAudioClips.Length)],
        SFX.DestroyedVolume * SFX.VolumeMultiplier(transform.position));

        Destroy(effect, 5f);
        Destroy(gameObject);
    }

    private Transform[] VerifyArryNulability(Transform[] firePositions)
    {
        return (firePositions == null) ? new Transform[] { } : firePositions;
    }
}
