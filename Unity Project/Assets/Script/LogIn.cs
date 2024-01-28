using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LogIn : MonoBehaviour
{
    [SerializeField] InputField nameField;
    [SerializeField] InputField passwordField;
    [SerializeField] Button logInBtn;
    [SerializeField] Text failedLogInText;

    Menu menu;
    SaveDataManager saveDataManager;

    public void CallLogIn()
    {
        menu = FindObjectOfType<Menu>();
        saveDataManager = FindObjectOfType<SaveDataManager>();

        StartCoroutine(LogInUser()); 
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator LogInUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.Url + "login", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + " User does not exist");
                failedLogInText.text = "User does not exist";
                //DBManager.UserName = nameField.text;
                //DBManager.getUserid.id = null;
                //saveDataManager.ReadData();
                //DBManager.SetGameDataToLocalJSON(saveDataManager.ReadData(), nameField.text);
                //menu.GotoBegining();
            }
            else
            {
                Debug.Log("User " + nameField.text + " successfully LogIn:" + www.downloadHandler.text);
                DBManager.UserName = nameField.text;
                DBManager.getUserid = JsonUtility.FromJson<DBManager.UserId>(www.downloadHandler.text);
                DBManager.HasServerConaction = true;
                yield return StartCoroutine(DBManager.GetUserStats(DBManager.Url + "user/" + DBManager.getUserid.id + "/stats"));
                menu.GotoBegining();
            }
        }
    }

    public void VerifyInputs()
    {
        logInBtn.interactable = (nameField.text.Length >= 1 && passwordField.text.Length >= 1 && nameField.text.Length <= 16 && passwordField.text.Length <= 16);
    }
}
