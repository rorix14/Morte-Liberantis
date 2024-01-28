using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss Path Config")]
public class MoveConfig : ScriptableObject
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] float moveSpeed = 2f;

    public GameObject EnemyPrefab
    {
        get { return enemyPrefab; }
    }
    public float MoveSpeed
    {
        get { return moveSpeed; }
    }
    public List<Transform> GetWaypoits()
    {
        var waveWaypoits = new List<Transform>();
        foreach (Transform child in pathPrefab.transform)
        {
            waveWaypoits.Add(child);
        }
        return waveWaypoits;
    }
}
