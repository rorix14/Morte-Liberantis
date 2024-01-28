using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using LitJson;

public class MultiplayerProcessor : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject invaderSpawnPoint;
    [SerializeField] GameObject playerInvader;
    RabbitMqService rabbitService;
    MultiplayerMessage inSpawnMessage;
    MultiplayerMessage playerForVerification;
    WorldSnapshotMessage worldSnapshotMessage;
    GameScession gameScession;
    float lastPlayerHealth;
    string[] subjects;
    bool hasLoaded;
    bool hasSpawned;
    GameObject invader;
    int currentScene;
    bool verifyInvader;
    bool hasInitialWorld;
    bool hasSentInitialWorldMessage;
    Coroutine coroutine;
    enum ROUTING_KEY
    {
        SPAWN, LIFE, INPUT, WORLD
    }

    delegate void AfeterInitialWorldMessage(WorldSnapshotMessage message);
    event AfeterInitialWorldMessage AfeterInitialWorldRescivedMessageEvent;
    public float LastPlayerHealth
    {
        get { return lastPlayerHealth; }
    }

    private void Awake()
    {
        if (!DBManager.HasServerConaction) return;

        rabbitService = new RabbitMqService(System.Enum.GetNames(typeof(ROUTING_KEY)));
        int numMultiplayerSessions = FindObjectsOfType<MultiplayerProcessor>().Length;

        if (numMultiplayerSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            hasLoaded = true;
            DontDestroyOnLoad(gameObject);
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (invaderSpawnPoint == null)
        {
            invaderSpawnPoint = GameObject.FindGameObjectWithTag("Invader Spawn");
        }
        gameScession = GetComponent<GameScession>();
    }

    void Start()
    {
        if (!DBManager.HasServerConaction) return;

        currentScene = SceneManager.GetActiveScene().buildIndex;

        rabbitService.subscribe(ROUTING_KEY.INPUT.ToString(), (ch, ea) =>
        {
            var playerRecived = JsonUtility.FromJson<MultiplayerMessage>(System.Text.Encoding.UTF8.GetString(ea.Body));

            if (DBManager.isInvader && playerRecived.playerId == DBManager.InvadedPlayerId && playerRecived.playerId != DBManager.getUserid.id)
            {
                inSpawnMessage = playerRecived;
            }

            if (!DBManager.isInvader && playerRecived.playerId == DBManager.InvaderId && playerRecived.playerId != DBManager.getUserid.id)
            {
                inSpawnMessage = playerRecived;
            }
            // ... process the message
            //ch.BasicAck(ea.DeliveryTag, false);
        });

        rabbitService.subscribe(ROUTING_KEY.WORLD.ToString(), (ch, ea) =>
        {
            var worldSnapshotMessage = JsonUtility.FromJson<WorldSnapshotMessage>(System.Text.Encoding.UTF8.GetString(ea.Body));

            if (!isMainPlayerMessage(worldSnapshotMessage.playerId))
            {
                return;
            }

            if (worldSnapshotMessage.initialMessage)
            {
                //print("recived for the first time with body " + System.Text.Encoding.UTF8.GetString(ea.Body));
                this.worldSnapshotMessage = worldSnapshotMessage;
                hasInitialWorld = true;
                // processInitialWorldMessage(worldSnapshotMessage);
            }
            else
            {
                //print("recived with body " + System.Text.Encoding.UTF8.GetString(ea.Body));
                this.worldSnapshotMessage = worldSnapshotMessage;
                //print("with body " + System.Text.Encoding.UTF8.GetString(ea.Body));
                //processWorldMessage(worldSnapshotMessage);
            }
        });

        rabbitService.subscribe(ROUTING_KEY.SPAWN.ToString(), (ch, ea) =>
        {
            var playerRecived = JsonUtility.FromJson<MultiplayerMessage>(System.Text.Encoding.UTF8.GetString(ea.Body));

            if (DBManager.isInvader && playerRecived.playerId == DBManager.InvadedPlayerId && playerRecived.playerId != DBManager.getUserid.id)
            {
                inSpawnMessage = playerRecived;
                hasSpawned = true;
            }

            if (!DBManager.isInvader && playerRecived.playerId != DBManager.getUserid.id)
            {
                playerForVerification = playerRecived;
                verifyInvader = true;
            }
            // ... process the message
            //ch.BasicAck(ea.DeliveryTag, false);
        });

        rabbitService.subscribe(ROUTING_KEY.LIFE.ToString(), (ch, ea) =>
        {
            var playerRecived = JsonUtility.FromJson<MultiplayerMessage>(System.Text.Encoding.UTF8.GetString(ea.Body));

            if (DBManager.isInvader && playerRecived.playerId == DBManager.InvadedPlayerId && playerRecived.playerId != DBManager.getUserid.id)
            {
                lastPlayerHealth = playerRecived.health;
            }

            if (!DBManager.isInvader && playerRecived.playerId == DBManager.InvaderId && playerRecived.playerId != DBManager.getUserid.id)
            {
                lastPlayerHealth = playerRecived.health;
            }
        });

        var outSpawnMessage = new MultiplayerMessage(ROUTING_KEY.SPAWN.ToString(), DBManager.getUserid.id, 0, 0, null, player.transform.position,
            player.transform.eulerAngles, SceneManager.GetActiveScene().buildIndex);

        rabbitService.publish(ROUTING_KEY.SPAWN.ToString(), JsonUtility.ToJson(outSpawnMessage, true));
        lastPlayerHealth = gameScession.MaxHealth;

        if (!DBManager.isInvader)
        {
            StartCoroutine(DBManager.updatePlayerPlayingStatus(DBManager.Url + "currently_palying/" + DBManager.getUserid.id + "/" + 1, st => { }));
        }
    }

    private bool isMainPlayerMessage(string playerId)
    {
        return DBManager.isInvader && playerId == DBManager.InvadedPlayerId;
    }

    public void publishWorldMessage(IDictionary<string, GameObject> enemies, bool initialMessage = false)
    {

        WorldSnapshotMessage message = new WorldSnapshotMessage();

        message.playerId = DBManager.getUserid.id;
        message.initialMessage = initialMessage;
        message.enemies = new WorldSnapshotMessage.Enemy[enemies.Values.Count];

        GameObject enemy;
        int index = 0;
        foreach (var enemyId in enemies.Keys)
        {
            enemy = enemies[enemyId];

            var messageEnemy = new WorldSnapshotMessage.Enemy();
            messageEnemy.enemyId = enemyId;
            messageEnemy.type = enemy.GetComponent<EnemyTypes>().enemyType;
            messageEnemy.position = enemy.transform.position;
            messageEnemy.rotation = enemy.transform.eulerAngles;
            messageEnemy.velocity = enemy.GetComponent<Rigidbody2D>().velocity;

            message.enemies.SetValue(messageEnemy, index);
            index++;
        }
        //print("publish: " + DBManager.getUserid.id);
        print("with sent body " + JsonUtility.ToJson(message, true));
        rabbitService.publish(ROUTING_KEY.WORLD.ToString(), JsonUtility.ToJson(message, true));
    }

    private void processWorldMessage(WorldSnapshotMessage message)
    {
        foreach (var enemy in message.enemies)
        {
            gameScession.UpdateEnemy(enemy.enemyId, enemy.type, enemy.position, enemy.rotation, enemy.velocity);
        }
        RemoveObsoleteEnemiesDuringPlay(message);
    }

    private void processInitialWorldMessage(WorldSnapshotMessage message)
    {
        foreach (var enemy in message.enemies)
        {
            gameScession.CorrelateEnemy(enemy.type, enemy.enemyId);
        }
        processWorldMessage(message);
        gameScession.RemoveInvaderObsuleteEnemies();
    }

    void InstanciateRightInvader(MultiplayerMessage playerRecived)
    {
        if (verifyInvader)
        {
            StartCoroutine(DBManager.GetDBInfo(DBManager.Url + "invader_id/" + DBManager.getUserid.id, result =>
            {
                DBManager.InvaderId = null;
                JsonData jsonvale = JsonMapper.ToObject(result);
                if (jsonvale["id"] != null && !jsonvale["id"].ToString().Equals("0"))
                {
                    DBManager.InvaderId = jsonvale["id"].ToString();
                }

                if (DBManager.InvaderId == playerRecived.playerId)
                {
                    inSpawnMessage = playerRecived;
                    hasSpawned = true;
                }
            }));
            verifyInvader = false;
        }
    }
    void Update()
    {
        if (!DBManager.HasServerConaction) return;

        InstanciateRightInvader(playerForVerification);
        if (hasSpawned && inSpawnMessage != null)
        {
            if (invader != null) Destroy(invader);
            if (DBManager.isInvader)
            {
                invader = Instantiate(playerInvader, inSpawnMessage.position.position, Quaternion.Euler(inSpawnMessage.position.rotation));
                //processInitialWorldMessage(worldSnapshotMessage);
            }
            else
            {
                invader = Instantiate(playerInvader, invaderSpawnPoint.transform.position, invaderSpawnPoint.transform.rotation);
                publishWorldMessage(gameScession.enemies, true);
                hasSentInitialWorldMessage = true;
            }
            hasSpawned = false;
        }

        if (invader == null)
        {
            hasSpawned = true;
        }

        var outInputMessage = new MultiplayerMessage(ROUTING_KEY.INPUT.ToString(), DBManager.getUserid.id, player.ControlTrow, player.VerticalAxis, player.PlayerInputs,
            player.transform.position, player.transform.eulerAngles, SceneManager.GetActiveScene().buildIndex);

        rabbitService.publish(ROUTING_KEY.INPUT.ToString(), JsonUtility.ToJson(outInputMessage, true));

        if (invader != null && inSpawnMessage != null)
        {
            DestroyRemnants();
            var playerInvaderComponent = invader.GetComponent<PlayerInvader>();
            playerInvaderComponent.MoveX = inSpawnMessage.horizontalAxis;
            playerInvaderComponent.VerticalAxis = inSpawnMessage.varticalAxis;
            playerInvaderComponent.InvaderInputs = inSpawnMessage.actions.Length == 0 ? null : inSpawnMessage.actions[0];
            playerInvaderComponent.CurrentHealth = lastPlayerHealth;
            WarpToPlace(inSpawnMessage);
        }

        player.PlayerInputs = null;

        if (inSpawnMessage != null)
        {
            MoveInvaderToPlayerScene();
            ReadWorld();
        }

        if (hasSentInitialWorldMessage && !DBManager.isInvader)
        {
            coroutine = StartCoroutine(SendWorldMessage());
            hasSentInitialWorldMessage = false;
        }

        inSpawnMessage = null;
        worldSnapshotMessage = null;
    }

    public void PublishHealthWhenHurt(float health)
    {
        //print("Publishing health " + health);
        var outInputMessage = new MultiplayerMessage(ROUTING_KEY.LIFE.ToString(), DBManager.getUserid.id, 0, 0, null,
           Vector3.zero, Vector3.zero, SceneManager.GetActiveScene().buildIndex, health);
        rabbitService.publish(ROUTING_KEY.LIFE.ToString(), JsonUtility.ToJson(outInputMessage, true));
    }

    void WarpToPlace(MultiplayerMessage content)
    {
        var distance = Math.Abs(Vector3.Distance(invader.transform.position, content.position.position));

        if (distance < 2f)
        {
            invader.transform.position = Vector3.Lerp(invader.transform.position, content.position.position, Time.deltaTime);
        }
        else
        {
            invader.transform.position = content.position.position;
        }
    }

    private void DestroyRemnants()
    {
        if (SceneManager.GetActiveScene().buildIndex != inSpawnMessage.position.sceneIndex)
        {
            var remenants = GameObject.FindGameObjectsWithTag("Invader");

            if (remenants == null)
            {
                return;
            }

            foreach (var remenat in remenants)
            {
                Destroy(remenat);
            }
        }
    }

    private void MoveInvaderToPlayerScene()
    {
        if (SceneManager.GetActiveScene().buildIndex != inSpawnMessage.position.sceneIndex && DBManager.isInvader)
        {
            AfeterInitialWorldRescivedMessageEvent -= processWorldMessage;
            SceneManager.LoadScene(inSpawnMessage.position.sceneIndex);
        }
    }

    private void ReadWorld()
    {
        if (worldSnapshotMessage == null) return;

        if (SceneManager.GetActiveScene().buildIndex == inSpawnMessage.position.sceneIndex && DBManager.isInvader)
        {
            if (worldSnapshotMessage.initialMessage)
            {
                processInitialWorldMessage(worldSnapshotMessage);
                AfeterInitialWorldRescivedMessageEvent += processWorldMessage;
                //hasInitialWorld = false;
            }
            if (!worldSnapshotMessage.initialMessage) AfeterInitialWorldRescivedMessageEvent?.Invoke(worldSnapshotMessage);
        }
    }
    IEnumerator SendWorldMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            publishWorldMessage(gameScession.enemies);
        }
    }

    private void ResetInvaderId()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", DBManager.InvadedPlayerId);
        form.AddField("invaderId", 0);
        CoroutineRunner.RunCoroutine(DBManager.UpdatePlayerInvaderId(DBManager.Url + "update_invader_id", form, res => { }));
        DBManager.InvadedPlayerId = null;
    }
    private void OnLevelWasLoaded(int level)
    {
        if (!DBManager.HasServerConaction) return;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        invaderSpawnPoint = GameObject.FindGameObjectWithTag("Invader Spawn");
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (coroutine != null) StopCoroutine(coroutine);
        //var outSpawnMessage = new MultiplayerMessage(ROUTING_KEY.SPAWN.ToString(), DBManager.getUserid.id, 0, 0, null, player.transform.position,
        // player.transform.eulerAngles, SceneManager.GetActiveScene().buildIndex);
        //rabbitService.publish(ROUTING_KEY.SPAWN.ToString(), JsonUtility.ToJson(outSpawnMessage, true));
    }

    private void OnDestroy()
    {
        if (!DBManager.HasServerConaction) return;

        if (hasLoaded)
        {
            print("Multi player processor has been destroyed");
            if (!DBManager.isInvader)
            {
                DBManager.InvaderId = null;
                CoroutineRunner.RunCoroutine(DBManager.updatePlayerPlayingStatus(DBManager.Url + "currently_palying/" + DBManager.getUserid.id + "/" + 0, st => { }));
            }
            else
            {
                ResetInvaderId();
            }
            rabbitService.destroy();
        }
    }

    void RemoveObsoleteEnemiesDuringPlay(WorldSnapshotMessage message)
    {
        var enemiesIds = new List<string>();
        var defetedEnemiesIds = new List<string>();

        foreach (var enemy in message.enemies)
        {
            enemiesIds.Add(enemy.enemyId);
        }

        foreach (var key in gameScession.playerToInvaderEnemyIdsCorrelation.Keys)
        {
            if (!enemiesIds.Contains(key)) defetedEnemiesIds.Add(gameScession.playerToInvaderEnemyIdsCorrelation[key]); ;
        }

        foreach (var id in defetedEnemiesIds)
        {
            if (gameScession.enemies.Keys.Contains(id))
            {
                Destroy(gameScession.enemies[id]);
                gameScession.enemies.Remove(id);
            }
        }
    }
}
