using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamage : MonoBehaviour
{
    [SerializeField] GameObject projectilePefab;
    [SerializeField] List<GameObject> fireDown;
    [SerializeField] List<GameObject> fireRight;
    [SerializeField] List<GameObject> fireLeft;
    //[SerializeField] List<GameObject>
    BossHealth bossHealth;
    SpriteRenderer imageColor;
    PathManager pathManager;
    float cooldown;
    float sumsummoning = 2f;
    float gain;
    int rotation = 90;
    bool doRotine;

    public enum State { frist, second, third, forth }
    [SerializeField] public State CurrentState;

    void Start()
    {
        pathManager = FindObjectOfType<PathManager>().GetComponent<PathManager>();
        bossHealth = FindObjectOfType<BossHealth>().GetComponent<BossHealth>();
        imageColor = GetComponent<SpriteRenderer>();

        //CurrentState = State.frist;
    }
    void Update()
    {
        gain += (0.7f * Time.deltaTime);
        if (gain <= 1) GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)((gain) * 255));

        sumsummoning -= Time.deltaTime;
        if (sumsummoning <= 0) DoFireByState();

        //UpdateState();
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

    void DoFireByState()
    {
        Vector3 rot;
        float rotZ = 90;
        Vector2 vel;
        float velX = 0;
        float velY = 10;

        float rotSum;

        switch (CurrentState)
        {
            case State.frist:
                foreach (var place in fireLeft)
                {
                    FireOne(1, place.transform.position, new Vector3(0, 0, 180), new Vector2(-10, 0));
                }
                FireOne(1, fireDown[0].transform.position, new Vector3(0, 0, 225), new Vector2(10 * Mathf.Cos(225 * Mathf.PI / 180), 10 * Mathf.Sin(225 * Mathf.PI / 180)));
                break;
            case State.second:
                foreach (var place in fireRight)
                {
                    FireOne(1, place.transform.position, new Vector3(0, 0, 0), new Vector2(10, 0));
                }
                foreach (var place in fireDown)
                {
                    FireOne(1, place.transform.position, new Vector3(0, 0, -90), new Vector2(0, -10));
                }
                FireOne(1, fireDown[fireDown.Count - 1].transform.position, new Vector3(0, 0, 315), new Vector2(10 * Mathf.Cos(315 * Mathf.PI / 180), 10 * Mathf.Sin(315 * Mathf.PI / 180)));
                break;
            case State.third:
                if (!doRotine)
                {
                    StartCoroutine(Fire());
                    doRotine = true;
                }
                break;
            case State.forth:
                for (int i = 0; i < 4; i++)
                {
                    rotSum = rotZ + rotation;

                    rot = new Vector3(0, 0, rotSum);

                    vel = new Vector2(10 * Mathf.Cos(rotSum * Mathf.PI / 180), 10 * Mathf.Sin(rotSum * Mathf.PI / 180));
                    FireOne(0, transform.position, rot, vel);

                    rotZ += 90;
                }

                rot = new Vector3(0, 0, rotation);
                vel = new Vector2(10 * Mathf.Cos(rotation * Mathf.PI / 180), 10 * Mathf.Sin(rotation * Mathf.PI / 180));
                FireOne(0, transform.position, rot, vel);
                rotation += 1;
                /*for (int i = 0; i < 4; i++)
                {
                    var prevVelX = velX;

                    rot = new Vector3(0, 0, rotZ);
                    vel = new Vector2(velX, velY);
                    FireOne(0, transform.position, rot, vel);
                    rotZ += 90;

                    velX = (velY != 0) ? -velY : 0;
                    velY = (velX != 0) ? 0 : prevVelX;
                }*/
                break;
        }
    }
    void FireOne(float coolDownTime, Vector3 position, Vector3 rotaion, Vector2 velocity)
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            GameObject projectile;
            projectile = Instantiate(projectilePefab, position, Quaternion.Euler(rotaion)) as GameObject;
            projectile.GetComponent<Rigidbody2D>().velocity = velocity;
            projectile.GetComponent<FireBall>().ProcessFieredShoot(position, rotaion);

            cooldown = coolDownTime;
        }
    }
    IEnumerator Fire()
    {
        do
        {
            GameObject[] projectile = { Instantiate(projectilePefab, fireRight[0].transform.position, Quaternion.Euler(new Vector3(0, 0, 315)))
            , Instantiate(projectilePefab, fireLeft[0].transform.position, Quaternion.Euler(new Vector3(0, 0, 225)))};
            projectile[0].GetComponent<Rigidbody2D>().velocity = new Vector2(10 * Mathf.Cos(315 * Mathf.PI / 180), 10 * Mathf.Sin(315 * Mathf.PI / 180));
            projectile[0].GetComponent<FireBall>().ProcessFieredShoot(fireRight[0].transform.position, new Vector3(0, 0, 315));
            projectile[1].GetComponent<Rigidbody2D>().velocity = new Vector2(10 * Mathf.Cos(225 * Mathf.PI / 180), 10 * Mathf.Sin(225 * Mathf.PI / 180));
            projectile[1].GetComponent<FireBall>().ProcessFieredShoot(fireLeft[0].transform.position, new Vector3(0, 0, 225));

            //Instantiate(projectilePefab, fireRight[0].transform.position, Quaternion.Euler(new Vector3(0, 0, 315))).GetComponent<Rigidbody2D>().velocity = new Vector2(10 * Mathf.Cos(315 * Mathf.PI / 180), 10 * Mathf.Sin(315 * Mathf.PI / 180));
            // Instantiate(projectilePefab, fireLeft[0].transform.position, Quaternion.Euler(new Vector3(0, 0, 225))).GetComponent<Rigidbody2D>().velocity = new Vector2(10 * Mathf.Cos(225 * Mathf.PI / 180), 10 * Mathf.Sin(225 * Mathf.PI / 180));
            foreach (var place in fireDown)
            {
                Instantiate(projectilePefab, place.transform.position, Quaternion.Euler(new Vector3(0, 0, -90))).GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);

                FireOne(0, place.transform.position, new Vector3(0, 0, -90), new Vector2(0, -10));
            }
            //FireOne(0, fireRight[0].transform.position, new Vector3(0, 0, 315), new Vector2(10 * Mathf.Cos(315 * Mathf.PI / 180), 10 * Mathf.Sin(315 * Mathf.PI / 180)));
            //FireOne(0, fireLeft[0].transform.position, new Vector3(0, 0, 225), new Vector2(10 * Mathf.Cos(225 * Mathf.PI / 180), 10 * Mathf.Sin(225 * Mathf.PI / 180)));
            yield return new WaitForSeconds(1f);
        } while (CurrentState == State.third);
    }
}
