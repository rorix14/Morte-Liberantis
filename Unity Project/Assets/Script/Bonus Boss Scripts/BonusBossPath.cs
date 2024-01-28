using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bonus Boss Path Config")]
public class BonusBossPath : ScriptableObject
{
    [SerializeField] GameObject[] pathPrefab;
    [SerializeField] float[] moveSpeed;

    public float[] MoveSpeed
    {
        get { return moveSpeed; }
    }
    public GameObject [] Paths
    {
        get { return pathPrefab; }
    }
    public List<Vector3> GetWaypoits(GameObject path)
    {
        var waveWaypoits = new List<Vector3>();
        foreach (Transform child in path.transform)
        {
            waveWaypoits.Add(child.position);
        }
        return waveWaypoits;
    }
}
