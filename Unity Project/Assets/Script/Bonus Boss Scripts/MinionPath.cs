using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionPath : MonoBehaviour
{
    WaveConfig waveConfig;
    List<Vector3> waypoints;
    float enemySpeed;
    int wayPointIndex = 0;
    IEnumerator ShootCourotine;
    void Start()
    {
        waypoints = waveConfig.GetWaypoits();
        enemySpeed = waveConfig.MoveSpeed;
    }
    void Update()
    {
        Move();
    }

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        this.waveConfig = waveConfig;
    }
   
    private void Move()
    {
        if (wayPointIndex <= waypoints.Count - 1)
        {
            var targetPos = waypoints[wayPointIndex];
            var movementThisFrame = enemySpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);

            if (transform.position == targetPos)
            {
                wayPointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
