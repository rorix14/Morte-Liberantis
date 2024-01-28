using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayOffLine : MonoBehaviour
{
    [SerializeField] InputField nameField;
    [SerializeField] Button playOffLineBtn;
    [SerializeField] Button registerOffLineUserBtn;

    Menu menu;
    SaveDataManager saveDataManager;

    public void PlayOffLineTrigger()
    {
        menu = FindObjectOfType<Menu>();
        saveDataManager = FindObjectOfType<SaveDataManager>();
        DBManager.UserName = nameField.text;
        DBManager.getUserid.id = null;
        //DBManager.SetGameDataToLocalJSON(saveDataManager.ReadData(), nameField.text);
        menu.GotoBegining();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }

    public void RegisterOffLineUser()
    {
        menu = FindObjectOfType<Menu>();
        saveDataManager = FindObjectOfType<SaveDataManager>();

        StartCoroutine(SaveOffLineUserInServer(DBManager.Url + "save_point", saveDataManager.SendJSONString()));
    }
    public void RegisterPreviousUser()
    {
        menu = FindObjectOfType<Menu>();
        saveDataManager = FindObjectOfType<SaveDataManager>();

        StartCoroutine(SaveOffLineUserInServer(DBManager.Url + "save_point", saveDataManager.SendJSONString()));
    }
    IEnumerator SaveOffLineUserInServer(string url, string jsonFile)
    {
        Debug.Log(jsonFile);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonFile);

        using (UnityWebRequest www = UnityWebRequest.Put(url, postData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
           
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                DBManager.SetGameDataToLocalJSON(saveDataManager.ReadData(), saveDataManager.ReadData().user.name);
                Debug.Log("On Save Update Server Error" + www.error);
                menu.GotoBegining();
            }
            else
            {
                Debug.Log("update sucessefull: " + www.downloadHandler.text);
                DBManager.getUserid = JsonUtility.FromJson<DBManager.UserId>(www.downloadHandler.text);
                DBManager.UserName = saveDataManager.ReadData().user.name;
                DBManager.HasServerConaction = true;
                yield return StartCoroutine(DBManager.GetUserStats(DBManager.Url + "user/" + DBManager.getUserid.id + "/stats"));
                menu.GotoBegining();
            }
        }
    }  
    public void VerifyInputs()
    {
        saveDataManager = FindObjectOfType<SaveDataManager>();

        playOffLineBtn.interactable = (nameField.text.Length >= 1 && nameField.text.Length <= 16);
        //registerOffLineUserBtn.interactable = (nameField.text.Length >= 1 && nameField.text.Length <= 16);
        //registerOffLineUserBtn.interactable = (saveDataManager.ReadData().user.name != null);
    }
}
