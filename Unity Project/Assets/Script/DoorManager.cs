using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    //public static DoorManager instance = null;

    private int currentDoorNumber;

    [SerializeField] GameObject player;
    [SerializeField] GameObject[] doorArray;
    [SerializeField] GameObject gameScession;
    [SerializeField] GameObject[] savePoints;
    private void Awake()
    {
        /*if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }*/
        int numGameSessions = FindObjectsOfType<DoorManager>().Length;

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
        if (doorArray.Length == 0)
        {
            doorArray = GameObject.FindGameObjectsWithTag("Door");
        }
        if (gameScession == null)
        {
            gameScession = GameObject.FindGameObjectWithTag("Game Scession");
        }
        if (savePoints.Length == 0)
        {
            savePoints = GameObject.FindGameObjectsWithTag("Save Point");
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            Destroy(gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        //print("i am have loaded");
        player = GameObject.FindGameObjectWithTag("Player");
        doorArray = GameObject.FindGameObjectsWithTag("Door");
        gameScession = GameObject.FindGameObjectWithTag("Game Scession");
        savePoints = GameObject.FindGameObjectsWithTag("Save Point");

        //switch (gameScession.GetComponent<GameScession>().IsDead)
        switch (DBManager.LoadInSavePoint)
        {
            case false:
                for (int i = 0; i < doorArray.Length; i++)
                {
                    if (doorArray[i].GetComponent<Door>().DoorNumber == currentDoorNumber)
                    {
                        player.transform.position = doorArray[i].GetComponent<Door>().ExitPosition;
                        player.transform.eulerAngles = doorArray[i].GetComponent<Door>().ExitAngle;
                        print("door:" + DBManager.LoadInSavePoint);
                    }
                }
                break;
            case true:
                foreach (GameObject savePoint in savePoints)
                {
                    if (savePoint.GetComponent<SavePoint>().SaveId == DBManager.SavePointId)
                    {
                        player.transform.position = savePoint.transform.position;
                        print("save point:" + DBManager.LoadInSavePoint);
                    }
                }
                gameScession.GetComponent<GameScession>().getAmmo = DBManager.AmmoAmought;
                gameScession.GetComponent<GameScession>().ProcessPlayerHealth(gameScession.GetComponent<GameScession>().MaxHealth);
                gameScession.GetComponent<GameScession>().AddToScore(0);
                DBManager.LoadInSavePoint = false;
               //gameScession.GetComponent<GameScession>().IsDead = false;
                break;
        }
    }
    public void LoadScene(int passedDoorNumber, int nextScene)
    {
        currentDoorNumber = passedDoorNumber;

        SceneManager.LoadScene(nextScene);
    }
}
