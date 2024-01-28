using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPathing : MonoBehaviour
{
    //MoveConfig mobeConfig;
    List<Transform> movepoints;
    float enemySpeed;
    int wayPointIndex = 0;
    BossHealth bossHealth;
    BossDamage bossDamage;

    [SerializeField] List<MoveConfig> moveConfigs;

    void Start()
    {
        bossHealth = FindObjectOfType<BossHealth>().GetComponent<BossHealth>();
        bossDamage = GetComponent<BossDamage>();
        if(movepoints == null || movepoints.Count == 0)
        {
            SetWaveConfig(moveConfigs[0]);
        }
        // transform.position = waypoints[wayPointIndex].transform.position;
    }
    void Update()
    {
        Move();
    }

    public void SetWaveConfig(MoveConfig waveConfig)
    {
        //mobeConfig = waveConfig;
        movepoints = waveConfig.GetWaypoits();
        enemySpeed = waveConfig.MoveSpeed;
    }
    private void Move()
    {
        if (wayPointIndex <= movepoints.Count - 1)
        {
            var targetPos = movepoints[wayPointIndex].transform.position;
            var movementThisFrame = enemySpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);

            if (transform.position == targetPos)
            {
                wayPointIndex++;
            }
        }
        else
        {
            wayPointIndex = 0;
        }
    }
}
