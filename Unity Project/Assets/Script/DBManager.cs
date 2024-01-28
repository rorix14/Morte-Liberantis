using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using LitJson;

public static class DBManager
{
    // CHANGE URL HERE WHITH DESIRED IP
   // private static string url = "http://10.72.119.151:8080/";
    private static string url = "http://localhost:8080/";
    private static string userName;
    private static List<string> upgrades = new List<string>();
    private static Characters[] characters;
    private static UserId userId = new UserId();
    private static UserStats userStats = new UserStats();
    private static bool questCompleated;
    private static int? lastSavedScene = null;
    private static int? savePointId = null;
    private static List<int> charactersIds = new List<int>();
    private static int ammoAmought;
    private static bool hasServerConection = false;
    private static bool loadInSavePoint = false;
    private static bool doubleJumpPWUp = false;
    private static bool dashPWUp = false;
    public static bool isInvader;
    private static string invadedPlayerId;
    private static string invaderId;

    public static string Url
    {
        get { return url; }
    }
    public static string InvadedPlayerId
    {
        get { return invadedPlayerId; }
        set { invadedPlayerId = value; }
    }
    public static string InvaderId
    {
        get { return invaderId; }
        set { invaderId = value; }
    }
    public static string UserName
    {
        get { return userName; }
        set { userName = value; }
    }
    public static bool DoubleJumpPWUp
    {
        get { return doubleJumpPWUp; }
    }
    public static bool DashPWUp
    {
        get { return dashPWUp; }
    }
    public static List<string> UpgradesNames
    {
        get { return upgrades; }
        //set { upgrades.Add(value); }
    }
    public static string AddToUpgardesList
    {
        set { upgrades.Add(value); }
    }
    public static Characters[] CharactersArry
    {
        get { return characters; }
    }
    public static UserId getUserid
    {
        get { return userId; }
        set { userId = value; }
    }
    public static bool QuestCompleated
    {
        get { return questCompleated; }
        set { questCompleated = value; }
    }
    public static int? LastSavedScene
    {
        get { return lastSavedScene; }
        set { lastSavedScene = value; }
    }
    public static int? SavePointId
    {
        get { return savePointId; }
        set { savePointId = value; }
    }
    public static int AmmoAmought
    {
        get { return ammoAmought; }
        set { if (value >= 0) ammoAmought = value; }
    }
    public static bool HasServerConaction
    {
        get { return hasServerConection; }
        set { hasServerConection = value; }
    }
    public static bool LoadInSavePoint
    {
        get { return loadInSavePoint; }
        set { loadInSavePoint = value; }
    }
    public static bool LoggedIn { get { return userName != null; } }

    public static void LogOut()
    {
        userName = null;
    }
    public static List<int> CharactersIds
    {
        get { return charactersIds; }
    }
    public static void AddToCharactersIdsList(int characterMeetId = 0)
    {
        if (CharactersArry != null)
        {
            for (int i = 0; i < CharactersArry.Length; i++)
            {
                if (!charactersIds.Contains(CharactersArry[i].id)) { charactersIds.Add(CharactersArry[i].id); }
            }
        }

        if (characterMeetId != 0 && !charactersIds.Contains(characterMeetId)) { charactersIds.Add(characterMeetId); }
    }

    public static void InitializePlayerUpgrades()
    {
        foreach (string upgrade in upgrades)
        {
            switch (upgrade)
            {
                case "double jump":
                    doubleJumpPWUp = true;
                    break;
                case "dash":
                    dashPWUp = true;
                    break;
            }
        };
    }

    public static void GetCharactersFromJsonArry(string jsonFromServer)
    {
        string jsonString = FixJson(jsonFromServer);
        characters = JsonHelper.FromJson<Characters>(jsonString);
    }
    public static string FixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }
    [Serializable]
    public class Characters
    {
        public string name;
        public string image;
        public int id;
    }

    [Serializable]
    public class UserId
    {
        public string id;
    }

    [Serializable]
    public class UserStats
    {
        public int ammoAmount;
        public List<string> upgradeNames = new List<string>();
        public int savePointIdentifier;
        public int savePointScene;
    }

    public static void SetGameDataToLocalJSON(SaveDataManager.PlayerData playerData, string playerName)
    {
        HasServerConaction = false;

        getUserid.id = playerData.user.id;
        UserName = playerName;

        AmmoAmought = playerData.ammo.amount;

        playerData.characterIds.ForEach(el => charactersIds.Add(el));

        upgrades.Clear();
        playerData.upgradeNames.ForEach(el => upgrades.Add(el));

        savePointId = playerData.savePoint.identifier;
        lastSavedScene = playerData.savePoint.scene;

        loadInSavePoint = true;

        if (!savePointId.HasValue && !lastSavedScene.HasValue)
        {
            loadInSavePoint = false;
        }

        doubleJumpPWUp = false;
        dashPWUp = false;
        InitializePlayerUpgrades();
    }

    public static void SetGameDataToDBInfo(UserStats userStats)
    {
        ammoAmought = userStats.ammoAmount;

        upgrades.Clear();
        userStats.upgradeNames.ForEach(el => upgrades.Add(el));
        savePointId = userStats.savePointIdentifier;
        lastSavedScene = userStats.savePointScene;

        loadInSavePoint = true;

        if (!savePointId.HasValue && !lastSavedScene.HasValue)
        {
            loadInSavePoint = false;
        }

        doubleJumpPWUp = false;
        dashPWUp = false;
        InitializePlayerUpgrades();
    }
    // SERVER METHODS 
    // get charactres the player found in game 
    public static IEnumerator GetFoundCharactrs()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url + "user/" + getUserid.id + "/characters"))
        {
            //Debug.Log("test id nullable: " + getUserid.id.Value);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error + " user does not exist");
            }
            else
            {
                Debug.Log("Found characters: " + webRequest.downloadHandler.text);

                GetCharactersFromJsonArry(webRequest.downloadHandler.text);

                // for testing, take out once character interactions are done
                //foreach (Characters number in CharactersArry)
                //{
                //    Debug.Log(number.id);
                //}
            }
        }
    }
    // insert character in DB
    public static IEnumerator InserCharacter(int characterId)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", getUserid.id);
        form.AddField("charactersId", characterId.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(url + "user_characters", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + " Server Error");
            }
            else
            {
                Debug.Log("Response from server: " + www.downloadHandler.text);
            }
        }
    }
    // get daily quest status
    public static IEnumerator getDailyQuestStatus(string characterId)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterId", characterId);

        using (UnityWebRequest www = UnityWebRequest.Post(url + "user/" + getUserid.id + "/" + DateTime.Now.ToString("dd_MM_yyyy"), form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + " Server Error");
            }
            else
            {
                Debug.Log("daily quest status: " + www.downloadHandler.text);
                // get quest completed status
                JsonData jsonvale = JsonMapper.ToObject(www.downloadHandler.text);
                QuestCompleated = jsonvale["dailyQuestStatus"].ToString().Equals("completed") ? true : false;
                //Debug.Log("is quest completed: " + questCompleated);
            }
        }
    }
    //on save point update DB with local json file 
    public static IEnumerator UpdatePlayerDataOnSavePoint(string url, string jsonFile)
    {
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonFile);

        using (UnityWebRequest www = UnityWebRequest.Put(url, postData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            Debug.Log(jsonFile);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("On Save Update Server Error" + www.error);
            }
            else
            {
                Debug.Log("data uploaded to server: " + www.downloadHandler.text);
            }
        }
    }
    // get from DB user ammo, last save point and upgrades collected
    public static IEnumerator GetUserStats(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error + " user does not exist");
            }
            else
            {
                Debug.Log("User stats: " + webRequest.downloadHandler.text);

                userStats = JsonUtility.FromJson<UserStats>(webRequest.downloadHandler.text);
                SetGameDataToDBInfo(userStats);

                foreach (string number in userStats.upgradeNames)
                {
                    Debug.Log(number);
                }
            }
        }
    }

  public static IEnumerator GetDBInfo(string url, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + "server error");
            }
            else
            {
                Debug.Log("Message received: " + www.downloadHandler.text);
                callBack(www.downloadHandler.text);
            }
        }
    }

    public static IEnumerator updatePlayerPlayingStatus(string url, Action<string> lamda)
    {
        using (UnityWebRequest www = UnityWebRequest.Put(url, System.Text.Encoding.UTF8.GetBytes("test")))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("On Save Update Server Error" + www.error);
            }
            else
            {
                Debug.Log("data uploaded to server: " + www.downloadHandler.text);

                JsonData jsonvale = JsonMapper.ToObject(www.downloadHandler.text);

                lamda(jsonvale["playingStatus"].ToString());
            }
        }
    }

    public static IEnumerator UpdatePlayerInvaderId(string url, WWWForm form, Action<string> callBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + " Server Error");
            }
            else
            {
                Debug.Log("Message received: " + www.downloadHandler.text);
                callBack(www.downloadHandler.text);
            }
        }
    }
}
