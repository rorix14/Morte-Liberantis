using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveBonusBoss : MonoBehaviour
{
    BonusBossPath currentPath;
    List<Vector3> movepoints;
    float enemySpeed;
    int pathIndex = 0;
    int wayPointIndex = 0;
    bool backwards;
    GameObject[] paths;
    float[] speed;
    Action<float>[] fireFunctions;
    float speedBonus;
    float reduceCooldown;

    void Update()
    {
        Move();
    }

    public void SetPathConfig(BonusBossPath waveConfig, Action<float>[] fireFunctions, float speedBonus = 1f, float reduceCooldown = 1f)
    {
        this.fireFunctions = fireFunctions;
        backwards = false;
        this.speedBonus = speedBonus;
        this.reduceCooldown = Mathf.Clamp(reduceCooldown, 0, 1);

        paths = new GameObject[waveConfig.Paths.Length];
        speed = new float[waveConfig.MoveSpeed.Length];

        Array.Copy(waveConfig.Paths, paths, waveConfig.Paths.Length);
        Array.Copy(waveConfig.MoveSpeed, speed, waveConfig.MoveSpeed.Length);

        currentPath = waveConfig;

        SetPathIndex(0);
    }

    private void SetPathIndex(int index)
    {
        pathIndex = index;
        wayPointIndex = 0;

        movepoints = currentPath.GetWaypoits(paths[pathIndex]);
        enemySpeed = speed[pathIndex] * speedBonus;

        if (pathIndex % 2 == 0 && backwards == true)
        {
            movepoints.Reverse();
        }
    }

    private void SetBackwards(bool backwards)
    {

        if (backwards != this.backwards)
        {
            Array.Reverse(paths);
            Array.Reverse(speed);
        }
        this.backwards = backwards;
    }

    private void Move()
    {
        if (wayPointIndex <= movepoints.Count - 1)
        {
            var targetPos = movepoints[wayPointIndex];
            var movementThisFrame = enemySpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);

            if (transform.position == targetPos)
            {
                wayPointIndex++;
            }
            FirePaternByPath(pathIndex);
        }
        else
        {
            if (pathIndex < currentPath.Paths.Length - 1)
            {
                SetPathIndex(pathIndex + 1);
            }
            else
            {
                SetBackwards(!backwards);
                SetPathIndex(0);
            }
        }
    }

    private void FirePaternByPath(int pathIndex)
    {
        if (fireFunctions != null)
        {
            fireFunctions[pathIndex](reduceCooldown);
        }
    }
}
