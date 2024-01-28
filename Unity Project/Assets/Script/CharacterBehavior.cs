using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CharacterBehavior : MonoBehaviour
{
    [SerializeField] GameObject InputInfo;
    [SerializeField] int characterId;
    public string characterName;
    enum DialogStates { hasNotMeet, hasMeet, hasDailyQuest }
    DialogStates currentDialogState;
    string dialogState = "has not meet";
    string[] notMetDialog = { "On meet line 1", "On meet line 2", "On meet line 3" };
    string[] hasMetDialog = { "Already meet line 1", "Already meet line 2", "Already meet line 3" };
    string[] onDailyQuestCompleated = { "On quest comlpeated line 1", "On quest comlpeated line 2", "On quest comlpeated line 3" };
    DialogueManager dialogueManager;
    bool hasMeet = false;
    bool startDialog = false;
    bool hasMadeQuery = false;
    float fadeInTexttime;
    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }
    // Update is called once per frame
    void Update()
    {
        ProssesCharacterInteractions();
    }
    void ProssesCharacterInteractions()
    {
        if (Input.GetButtonDown("Interact") && GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            ProccessMetCharacter();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        dialogState = dialogueManager.EndDialog(dialogState);
        startDialog = false;
        InputInfo.SetActive(false);
        fadeInTexttime = 0f;
    }
    void ProccessMetCharacter()
    {
        //if (DBManager.CharactersArry == null) { return; }
        if (DBManager.CharactersArry != null)
        {
            for (int i = 0; i < DBManager.CharactersArry.Length; i++)
            {
                if (characterId == (DBManager.CharactersArry[i].id))
                {
                    hasMeet = true;
                    dialogState = "default";
                    //DBManager.AddToCharactersIdsList();
                    Debug.Log("Already meet: " + DBManager.CharactersArry[i].name);
                }
            }
        }
        else
        {
            for (int i = 0; i < DBManager.CharactersIds.Count; i++)
            {
                if (characterId == (DBManager.CharactersIds[i]))
                {
                    hasMeet = true;
                    dialogState = "default";
                    //DBManager.AddToCharactersIdsList();
                    Debug.Log("Already meet id: " + DBManager.CharactersIds[i]);
                }
            }
        }
        DBManager.AddToCharactersIdsList(characterId);
        if (!hasMeet)
        {
            Debug.Log("Not meet: " + characterName);
            if (DBManager.HasServerConaction)
            {
                StartCoroutine(DBManager.InserCharacter(characterId));
            }
            print("test error string: " + characterId);
            hasMeet = true;
        }

        StartCoroutine(WaitForRequest());
    }
    IEnumerator WaitForRequest()
    {
        if (!hasMadeQuery)
        {
            yield return DBManager.getDailyQuestStatus(characterId.ToString());
            dialogState = (DBManager.QuestCompleated) ? "has daily quest" : dialogState;
        }

        hasMadeQuery = true;

        ProcessDialog(dialogState);
    }
    void ManageDialog(string[] dialog)
    {
        switch (startDialog)
        {
            case true:
                dialogueManager.DisplayNextSentence(dialogState);
                break;
            case false:
                dialogueManager.StartDialogue(characterName, dialog, dialogState);
                InputInfo.SetActive(false);
                startDialog = true;
                break;
        }
    }
    void ProcessDialog(string dialogState)
    {
        switch (dialogState)
        {
            case "has not meet":
                ManageDialog(notMetDialog);
                break;
            case "default":
                ManageDialog(hasMetDialog);
                break;
            case "has daily quest":
                ManageDialog(onDailyQuestCompleated);
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!startDialog)
        {
            InputInfo.SetActive(true);
            fadeInTexttime += (0.5f * Time.deltaTime);
            if (fadeInTexttime <= 1) InputInfo.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, (byte)(fadeInTexttime * 255));
        } 
    }
    /* IEnumerator InserCharacter()
     {
         WWWForm form = new WWWForm();
         // change when raedy for actual game
         form.AddField("userId", "5");
         form.AddField("charactersId", characterId.ToString());

         using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:8080/user_characters", form))
         {
             yield return www.SendWebRequest();

             if (www.isNetworkError || www.isHttpError)
             {
                 Debug.Log(www.error + " Server Error");
             }
             else
             {
                 Debug.Log("Responce from server: " + www.downloadHandler.text);
             }
         }
     }*/
}
