using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    BossHealth bossHealth;
    [SerializeField] List<GameObject> bossess;
    bool hasSpawned;
    [SerializeField] List<MoveConfig> moveConfigs;

    void Start()
    {
        bossHealth = FindObjectOfType<BossHealth>().GetComponent<BossHealth>();
        bossess.Add(GameObject.FindGameObjectWithTag("Boss"));
    }
    private void Update()
    {
        if (bossHealth.BossHP <= 0)
        {
            return;
        }

        GameObject boss = bossess[0];
        BossPathing bossPathing = boss.GetComponent<BossPathing>();
        BossDamage bossDamage = boss.GetComponent<BossDamage>();


        switch (bossHealth.BossHP / bossHealth.MaxHealth)
        {
            case 0.7f:
                bossDamage.CurrentState = BossDamage.State.second;
                bossPathing.SetWaveConfig(moveConfigs[1]);
                break;
            case 0.5f:
                bossDamage.CurrentState = BossDamage.State.third;
                bossPathing.SetWaveConfig(moveConfigs[2]);
                break;
            case 0.3f:
                bossDamage.CurrentState = BossDamage.State.forth;
                bossPathing.SetWaveConfig(moveConfigs[3]);
                if (!hasSpawned)
                {
                    SpawnBoss(moveConfigs[2], BossDamage.State.third);
                    hasSpawned = true;
                }
                break;
        }
    }

    private GameObject SpawnBoss(MoveConfig pathConfig, BossDamage.State initialState)
    {
        GameObject newEnemy = Instantiate(pathConfig.EnemyPrefab, pathConfig.GetWaypoits()[0].position, Quaternion.identity) as GameObject;
        newEnemy.GetComponent<BossPathing>().SetWaveConfig(pathConfig);
        newEnemy.GetComponent<BossDamage>().CurrentState = initialState;

        bossess.Add(newEnemy);

        return newEnemy;
    }
    /*
    [SerializeField] List<MoveConfig> moveConfigs;
    [SerializeField] int staringWave = 0;
    GameObject boss;
    GameObject bosshelper;
    BossHealth bossHealth;
    int index = 1;
    bool hasSpawned;
    //public enum State { frist, second, third, forth }
    //State currentState;
    //public State CurrentState
    //{
    //    get { return currentState; }
    //}
    void Awake()
    {
        boss = SpawnBoss(moveConfigs[0]);
    }
    void Start()
    {
        //currentState = State.frist;
        bossHealth = FindObjectOfType<BossHealth>().GetComponent<BossHealth>();
        //SpawnAllWaves();
    }
    private void Update()
    {
        ChangePath();
    }
    //private GameObject SpawnBoss(MoveConfig pathConfig)
    //{
    //    GameObject newEnemy = Instantiate(pathConfig.EnemyPrefab, pathConfig.GetWaypoits()[0].position, Quaternion.identity) as GameObject;
    //    newEnemy.GetComponent<BossPathing>().SetWaveConfig(pathConfig);
    //    return newEnemy;
    //}
    //void ChangePath()
    //{

    //    switch (bossHealth.BossHP / bossHealth.MaxHealth)
    //    {
    //        case 0.7f:
    //            currentState = State.second;
    //            boss.GetComponent<BossPathing>().SetWaveConfig(moveConfigs[1]);
    //            break;
    //        case 0.5f:
    //            currentState = State.third;
    //            boss.GetComponent<BossPathing>().SetWaveConfig(moveConfigs[2]);
    //            break;
    //        case 0.3f:
    //            currentState = State.forth;
    //            boss.GetComponent<BossPathing>().SetWaveConfig(moveConfigs[0]);
    //            if (!hasSpawned)
    //            {
    //                bosshelper = SpawnBoss(moveConfigs[2]);
    //                hasSpawned = true;
    //            }
    //            break;
    //    }
    //}
    private void SpawnAllWaves()
    {
        for (int i = staringWave; i < moveConfigs.Count; i++)
        {
            //print("Move config:" + waveConfigs.Count);
            var currentWave = moveConfigs[i];
            SpawnBoss(currentWave);
        }
    }
    */
}
