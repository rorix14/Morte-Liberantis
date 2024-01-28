using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> minionWaves;
    GameObject minion;

    IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (true);
    }
    private IEnumerator SpawnAllWaves()
    {
        for (int i = 0; i < minionWaves.Count; i++)
        {
            var currentWave = minionWaves[i];
            yield return StartCoroutine(SpawnAllEnemies(currentWave));
        }
    }

    private IEnumerator SpawnAllEnemies(WaveConfig waveConfig)
    {
        var newEnemy = Instantiate(waveConfig.EnemyPrefab, waveConfig.GetWaypoits()[0], Quaternion.identity);
        newEnemy.GetComponent<MinionPath>().SetWaveConfig(waveConfig);
        minion = newEnemy;

        yield return new WaitUntil(IsMinionDestroyed);
    }

    bool IsMinionDestroyed()
    {
        return minion == null;
    }
}
