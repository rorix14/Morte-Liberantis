using System;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public partial class SaveDataManager : MonoBehaviour
{
    //string fileName = "data.json";
    //string path;
    ///PlayerData playerData;
    //Quest q1;
    //Quest q2;
    //AcessJSON<TestJSON> acessJSON = new AcessJSON<TestJSON>();
    // DBManager.UserId dbTest;
    // Start is called before the first frame update
    void Start()
    {

        //path = Application.persistentDataPath + "/" + fileName;
        //Debug.Log(path);
        //testJSON.userId = 4;
        //testJSON.ammo = 50;

        /*  q1 = new Quest();
          q1.description = "quest 1";
          q1.id = 1;

          q2 = new Quest();
          q2.description = "quest 2";
          q2.id = 2;

          testJSON.quest.Add(q1);
          testJSON.quest.Add(q2);*/

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Melle Attack"))
        {
            //SaveData();
        }
        if (Input.GetButtonDown("Jump"))
        {
            //ReadData();
        }

    }

    // public void SaveData(int scene, int savePointId, List<int> upgradeId)
    public string SaveData(string id, string name, int ammoAmount, int savePointIdentifier, int currentScene, List<int> characterIds, List<string> upgardesNames)
    // public void SaveData()
    {
        string fileName = "data.json";
        string path;

        path = Application.persistentDataPath + "/" + fileName;
        //print(path);
        PlayerData playerData = new PlayerData();
        User user = new User();
        Ammo ammo = new Ammo();
        SavePoint savePoint = new SavePoint();

        user.id = id;
        user.name = name;

        ammo.id = 1;
        ammo.amount = ammoAmount;

        savePoint.identifier = savePointIdentifier;
        savePoint.scene = currentScene;

        playerData.user = user;
        playerData.ammo = ammo;
        characterIds.ForEach(el => playerData.characterIds.Add(el));
        upgardesNames.ForEach(el => playerData.upgradeNames.Add(el));
        playerData.savePoint = savePoint;

        string contents = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path, contents);

        return contents;
        //playerData.savePoint.Add(savePoint);
        /*AcessJSON<TestJSON> test = new AcessJSON<TestJSON>();
        test.Items = testJSON;
        string contents = JsonUtility.ToJson(test, true);*/
    }
    public PlayerData ReadData()
    {
        string fileName = "data.json";
        string path;
        path = Application.persistentDataPath + "/" + fileName;
        PlayerData playerData = new PlayerData();

        Debug.Log(path);
        try
        {
            if (File.Exists(path))
            {
                string contents = File.ReadAllText(path);
                playerData = JsonUtility.FromJson<PlayerData>(contents);
                //acessJSON = JsonUtility.FromJson<AcessJSON<TestJSON>>(contents);
                //testJSON =  acessJSON.Items;
                //acessJSON.items.GetType
                Debug.Log("test from json: " + playerData.user.name + ", "+ playerData.savePoint.scene);
            }
            else
            {
                Debug.Log("Unable to read data from the save file \n File may not exist");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error" + e.Message);
        }
        return playerData;
    }

    public string SendJSONString()
    {
        string fileName = "data.json";
        string path;
        string contents = null;
        path = Application.persistentDataPath + "/" + fileName;

        try
        {
            if (File.Exists(path))
            {
                contents = File.ReadAllText(path);
                return contents;
            }
            else
            {
                Debug.Log("Unable to read data from the save file \n File may not exist");
                return contents;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error" + e.Message);
            return contents;
        }
       
    }

    [Serializable]
    public class PlayerData
    {
        public User user;
        public Ammo ammo;
        public List<int> characterIds = new List<int>();
        public List<string> upgradeNames = new List<string>();
        public SavePoint savePoint;
        //public List<SavePoint> savePoint = new List<SavePoint>();
        //public List<Upgrades> upgrades = new List<Upgrades>();
    }
    [Serializable]
    public class User
    {
        public string id;
        public string name;
    }
    [Serializable]
    public class Ammo
    {
        public int id;
        public int amount;
    }
    [Serializable]
    public class SavePoint
    {
        public int identifier;
        public int scene;
    }
    // json exemple
    /*const data = {
     "user": {
         "id": 1,
         "name": "Pedro"
     },
     "ammo": {
         "id": 1,
         "amount": 60
     },
     "characterIds": [
         1,
         2,
         3
     ],
     "upgradeNames": [
         "double jump"
     ],
     "savePoint": {
         "identifier": 1,
         "scene": 1
     }
 }*/
    /*[Serializable]
    public class Upgrades
    {
        public int upgradeId;
        public int identifier;
    }*/
    /* [Serializable]
     public class AcessJSON<T>
     {
         public T Items;
     }*/
}
