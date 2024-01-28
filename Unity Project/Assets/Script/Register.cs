using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
//using UnityEngine.JSONSerializeModule;
using LitJson;
public class Register : MonoBehaviour
{
    [SerializeField] InputField nameField;
    [SerializeField] InputField passwordField;
    [SerializeField] Button registerBtn;
    [SerializeField] Text failedRegistrationText;
    Menu menu;

    public void CallRegister()
    {
        menu = FindObjectOfType<Menu>();

        StartCoroutine(RegisterUser());
    }

    IEnumerator RegisterUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.Url + "register", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + " User already exists");
                failedRegistrationText.text = "User already exists";
            }
            else
            {
                Debug.Log("User " + nameField.text + " successfully registered:" + www.downloadHandler.text);
                DBManager.UserName = nameField.text;
                DBManager.getUserid = JsonUtility.FromJson<DBManager.UserId>(www.downloadHandler.text);
                DBManager.HasServerConaction = true;
                menu.GotoBegining();
            }
        }
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }

    public void VerifyInputs()
    {
        registerBtn.interactable = (nameField.text.Length >= 1 && passwordField.text.Length >= 1 && nameField.text.Length <= 16 && passwordField.text.Length <= 16);
    }

    /* IEnumerator GetRequest(string uri)
     {
         using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
         {
             // Request and wait for the desired page.
             yield return webRequest.SendWebRequest();

            // string[] pages = uri.Split('/');
            //int page = pages.Length - 1;

             if (webRequest.isNetworkError)
             {
                 Debug.Log(webRequest.error+" user does not exist");
             }
             else
             {
                 //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                 //string[] test = webRequest.downloadHandler.text.Split(':');             
                 Processjson(webRequest.downloadHandler.text);
             }
         }
     }
     private void Start()
     {
         StartCoroutine(GetRequest("http://localhost:8080/test/pe"));

     }
      public class parseJSON
     {
         public string name;
         public int id;
     }*/
    /* private void Processjson(string jsonString)
     {
         JsonData jsonvale = JsonMapper.ToObject(jsonString);
         parseJSON parsejson;
         parsejson = new parseJSON();
         parsejson.name = jsonvale["name"].ToString();
         parsejson.id = System.Convert.ToInt32(jsonvale["id"].ToString());
         Debug.Log("Name in string: " + parsejson.name + ", and id as int: " + parsejson.id);
     }*/
}
