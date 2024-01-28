using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;
using System;
using System.Linq;

public class GameScession : MonoBehaviour
{
    //public static GameScession instance = null;
    int playerLives = DBManager.isInvader ? 1 : 1000;
    [SerializeField] int ammo;
    [SerializeField] Text livesText;
    [SerializeField] Text scoreText;
    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image dashDisplayImage;
    [SerializeField] GameObject[] enemiesPrefabs;
    [HideInInspector] [SerializeField] GameObject player;
    MultiplayerProcessor multiplayerProcessor;
    public bool isPuased = false;
    float maxHealth = DBManager.isInvader ? 150 : 500;
    [SerializeField] float health;

    public IDictionary<string, string> playerToInvaderEnemyIdsCorrelation;
    public IDictionary<string, GameObject> enemies;

    public int getAmmo
    {
        get { return ammo; }
        set { if (value >= 0) ammo = value; }
    }
    public bool getIsPaused
    {
        get { return isPuased; }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
    }
    public float PlayerHealth
    {
        get { return health; }
        set { health = value; }
    }
    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameScession>().Length;
        multiplayerProcessor = GetComponent<MultiplayerProcessor>();
        playerToInvaderEnemyIdsCorrelation = new Dictionary<string, string>();
        enemies = new Dictionary<string, GameObject>();

        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        //if (DBManager.AmmoAmought)
        //StartCoroutine(DBManager.GetUserStats("http://localhost:8080/user/" + DBManager.getUserid.id + "/stats"));
        ammo = DBManager.AmmoAmought;

        findAllEnemies();
        //print(enemies.Count + " enemies");
    }

    void findAllEnemies()
    {
        var id = 0;
        var existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in existingEnemies)
        {
            enemies.Add(id.ToString(), enemy);
            id++;
        }
    }

    public void UpdateEnemy(string playerEnemyId, string enemyType, Vector3 position, Vector3 rotation, Vector2 velocity)
    {
        string enemyId = playerToInvaderEnemyIdsCorrelation[playerEnemyId];

        if (!enemies.Keys.Contains(enemyId))
        {
            foreach (var enemyPrefab in enemiesPrefabs)
            {
                if (enemyPrefab.GetComponent<EnemyTypes>().enemyType == enemyType)
                {
                    var newEnemy = Instantiate(enemyPrefab, position, Quaternion.Euler(rotation));
                    newEnemy.GetComponent<Rigidbody2D>().velocity = velocity;
                    enemies.Add(enemyId, newEnemy);
                }
            }
        }
        else
        {
            GameObject enemy = enemies[enemyId];
            enemy.transform.position = position;
            enemy.transform.eulerAngles = rotation;
            enemy.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

    public void CorrelateEnemy(string enemyType, string playerEnemyId)
    {
        if (playerToInvaderEnemyIdsCorrelation.Keys.Contains(playerEnemyId))
        {
            print("containns ids: " + playerEnemyId);
            return;
        }

        GameObject enemy;
        foreach (var enemyId in enemies.Keys)
        {
            enemy = enemies[enemyId];
            if (enemy.GetComponent<EnemyTypes>().enemyType == enemyType &&
                !playerToInvaderEnemyIdsCorrelation.Values.Contains(enemyId) && !playerToInvaderEnemyIdsCorrelation.Keys.Contains(playerEnemyId))
            {
                playerToInvaderEnemyIdsCorrelation.Add(playerEnemyId, enemyId);
            }
        }

        print("correlations are ");
        foreach (KeyValuePair<string, string> kvp in playerToInvaderEnemyIdsCorrelation)
        {
            print("Corrulate key:" + kvp.Key + ", value: " + kvp.Value);
        }
    }

    public void RemoveInvaderObsuleteEnemies()
    {
        var keysToRemove = new List<string>();
        foreach (var key in enemies.Keys)
        {
            if (!playerToInvaderEnemyIdsCorrelation.Values.Contains(key))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var keyToRemove in keysToRemove)
        {
            Destroy(enemies[keyToRemove]);
            enemies.Remove(keyToRemove);
        }
    }

    void Start()
    {
        //health = maxHealth;
        ProcessPlayerHealth(maxHealth);
        livesText.text = playerLives.ToString();
        scoreText.text = ammo.ToString();
        // Comented for debuguing...
        StartCoroutine(DBManager.GetFoundCharactrs());
    }

    void Update()
    {
        healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, health / maxHealth, 7.5f * Time.deltaTime);

        if (DBManager.DashPWUp) DisplayDashCooldown();


        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            Destroy(gameObject);
        }
        PauseGame();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        var enemyKey = enemies.FirstOrDefault(x => x.Value == enemy).Key;
        if (enemyKey != null) enemies.Remove(enemyKey);
    }

    private void OnLevelWasLoaded(int level)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerToInvaderEnemyIdsCorrelation = new Dictionary<string, string>();
        enemies = new Dictionary<string, GameObject>();
        findAllEnemies();
    }
    public void AddToScore(int pointsToAdd)
    {
        //DBManager.AmmoAmought += pointsToAdd;
        ammo += pointsToAdd;
        scoreText.text = ammo.ToString();
    }
    public void updateScore()
    {
        // REVIEW
        try
        {
            scoreText.text = ammo.ToString();
        }
        catch (Exception e)
        {
            Debug.Log("Exception caught. " + e);
        }

    }
    public float ProcessPlayerHealth(float damageTaken)
    {
        health += damageTaken;

        if (health >= maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            health = 0;
          StartCoroutine(ProcessPlayerDeath());
        }

        if (DBManager.HasServerConaction) GetComponent<MultiplayerProcessor>().PublishHealthWhenHurt(health);

        return health;
    }
    public void DisplayDashCooldown()
    {
        float dif = Math.Abs(player.GetComponent<Player>().NextDash - Time.time);

        dashDisplayImage.enabled = true;
        if (Time.time > player.GetComponent<Player>().NextDash)
        {
            dashDisplayImage.fillAmount = 1f;
            dashDisplayImage.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            dashDisplayImage.fillAmount = Mathf.Lerp(dashDisplayImage.fillAmount, Mathf.Abs(-1 + (dif / player.GetComponent<Player>().CoolDownDash)), 100f * Time.deltaTime);
            dashDisplayImage.color = new Color32(255, 255, 255, 100);
        }
    }
    public IEnumerator ProcessPlayerDeath()
    {
        yield return new WaitForSeconds(1.5f);

        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    private void TakeLife()
    {
        DBManager.LoadInSavePoint = true;
        playerLives--;
        Destroy(player);

        if (DBManager.LastSavedScene.HasValue)
        {
            SceneManager.LoadScene(DBManager.LastSavedScene.Value);
        }
        else
        {
            SceneManager.LoadScene(7);
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        // IsDead = true;
        livesText.text = playerLives.ToString();
        healthBarImage.fillAmount = 1f;
    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(player);
        Destroy(gameObject);
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPuased = false;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void PauseGame()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (!isPuased)
            {
                pauseMenuCanvas.SetActive(true);
                Time.timeScale = 0;
                isPuased = true;
            }
            else
            {
                ResumeGame();
            }
        }
    }
    public void GotoMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}