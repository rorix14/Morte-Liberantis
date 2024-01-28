using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHealth : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [HideInInspector] [SerializeField] GameObject boss;
    [SerializeField] float maxHealth = 2000f;
    [SerializeField] float health;
    [SerializeField] GameObject destroyEffect;
    GameObject[] bossess;
    AudioSource bossTheme;
    public float MaxHealth
    {
        get { return maxHealth; }
    }
    public float BossHP
    {
        get { return health; }
        set { health = value; }
    }
    void Start()
    {
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("Boss");
        }
        bossTheme = GetComponent<AudioSource>();
        health = maxHealth;
    }
    void Update()
    {
        healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, health / maxHealth, 7.5f * Time.deltaTime);

        if(health <= 0)
        {
            bossTheme.volume -= (0.2f * Time.deltaTime);
        }
    }
    public float ProcessBossHealth(float damageTaken)
    {
        if (health >= maxHealth)
        {
            health = maxHealth;
        }

        health += damageTaken;

        if (health <= 0)
        {
            health = 0;
            bossess = GameObject.FindGameObjectsWithTag("Boss");
            foreach (var boss in bossess)
            {
                GameObject effect = Instantiate(destroyEffect, boss.transform.position, boss.transform.rotation);
                Destroy(effect, 10f);
                Destroy(boss);
            } 
        }
        switch (health / maxHealth)
        {
            case 0.7f:
                healthBarImage.color = new Color32(146, 125, 11, 255);
                break;
            case 0.5f:
                healthBarImage.color = new Color32(200, 110, 7, 255);
                break;
            case 0.3f:
                healthBarImage.color = new Color32(200, 19, 7, 255);
                break;
        }

        return health;
    }
}
