using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float maxHealth = 200f;
    [SerializeField] float shootVelocity = 1000f;
    [SerializeField] float timeBeteewnShots = 0.2f;
    [SerializeField] FireRoutines fireRoutine;
    [SerializeField] GameObject missilePrfab;
    enum FireRoutines { Eça, Ramalho, Guerra };
    float? minionHealth;

    public GameObject EnemyPrefab
    {
        get { return enemyPrefab; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
    }

    public float? MinionHealth
    {
        get { return minionHealth; }
        set { minionHealth = value; }
    }

    public Func<Transform[][], EnemySFX, GameScession, IEnumerator> FieringRutine
    {
        get
        {
            switch (fireRoutine)
            {
                case FireRoutines.Eça:
                    return EçaFieringRoutine;
                case FireRoutines.Ramalho:
                    return RamalhoFieringRoutine;
                case FireRoutines.Guerra:
                    return GuerraFieringRoutine;
                default:
                    return null;
            }
        }
    }

    public void SetToMaxHealth()
    {
        minionHealth = maxHealth;
    }

    public List<Vector3> GetWaypoits()
    {
        var waveWaypoits = new List<Vector3>();
        foreach (Transform child in pathPrefab.transform)
        {
            waveWaypoits.Add(child.position);
        }
        return waveWaypoits;
    }

    IEnumerator EçaFieringRoutine(Transform[][] fieringPositions, EnemySFX SFX, GameScession gameScession)
    {
        while (true)
        {
            foreach (var firePosition in fieringPositions[0])
            {
                Fire(firePosition.position, new Vector3(0, 0, 270), new Vector2(0, shootVelocity) * -1, SFX, gameScession);
                yield return new WaitForSeconds(timeBeteewnShots);
            }
        }
    }

    IEnumerator RamalhoFieringRoutine(Transform[][] fieringPositions, EnemySFX SFX, GameScession gameScession)
    {
        while (true)
        {
            FireByPoints(fieringPositions[0], -20, SFX, gameScession);
            FireByPoints(fieringPositions[1], 20, SFX, gameScession);

            yield return new WaitForSeconds(timeBeteewnShots);
            FireByPoints(fieringPositions[2], 0, SFX, gameScession);

            yield return new WaitForSeconds(timeBeteewnShots);
        }
    }

    IEnumerator GuerraFieringRoutine(Transform[][] fieringPositions, EnemySFX SFX, GameScession gameScession)
    {
        //CoroutineRunner.RunCoroutine(test(fieringPositions[0], 180, SFX, gameScession));
        //CoroutineRunner.RunCoroutine(test(fieringPositions[1], 0, SFX, gameScession));
        //Parallel.Invoke(() => tt.te(), () => { });
        while (true)
        {
            var rot = 0;
            foreach (var item in fieringPositions)
            {
                rot += 180;
                foreach (var firePosition in item)
                {
                    Vector2 vel = new Vector2(shootVelocity * Mathf.Cos(rot * Mathf.PI / 180), shootVelocity * Mathf.Sin(rot * Mathf.PI / 180));

                    Fire(firePosition.position, new Vector3(0, 0, rot), vel, SFX, gameScession);
                    yield return new WaitForSeconds(timeBeteewnShots);
                }
            }
            yield return new WaitForSeconds(timeBeteewnShots * 4);
        }
    }

    void FireByPoints(Transform[] fiaringPositions, float rotToAdd, EnemySFX SFX, GameScession gameScession)
    {
        var rot = 270f;

        foreach (var transform in fiaringPositions)
        {
            rot += rotToAdd;
            Vector2 vel = new Vector2(shootVelocity * Mathf.Cos(rot * Mathf.PI / 180), shootVelocity * Mathf.Sin(rot * Mathf.PI / 180));

            Fire(transform.position, new Vector3(0, 0, rot), vel, SFX, gameScession);
        }
    }
    void Fire(Vector3 position, Vector3 rotation, Vector2 velocity, EnemySFX SFX, GameScession gameScession)
    {
        GameObject projectile;
        projectile = Instantiate(missilePrfab, position, Quaternion.Euler(rotation));
        projectile.GetComponent<Rigidbody2D>().velocity = velocity * Time.deltaTime;
        projectile.GetComponent<SpriteRenderer>().color = new Color32(14, 255, 93, 255);
        projectile.GetComponent<FireBall>().ProcessFieredShoot(position, rotation, true);
        gameScession.GetComponent<AudioSource>().PlayOneShot(SFX.ShootAudioClips[UnityEngine.Random.Range(0, SFX.ShootAudioClips.Length)], SFX.ShootVolume);
    }
    // FOR TESTING PURPOSES
    IEnumerator test(Transform[] fieringPositions, float rot, EnemySFX SFX, GameScession gameScession)
    {
        Debug.Log("whow many times m i called");
        while (true)
        {
            foreach (var firePosition in fieringPositions)
            {
                Vector2 vel = new Vector2(shootVelocity * Mathf.Cos(rot * Mathf.PI / 180), shootVelocity * Mathf.Sin(rot * Mathf.PI / 180));

                Fire(firePosition.position, new Vector3(0, 0, rot), vel, SFX, gameScession);
                yield return new WaitForSeconds(timeBeteewnShots);
            }
        }
    }

}
