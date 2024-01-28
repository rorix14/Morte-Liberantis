using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossDialog : MonoBehaviour
{
    [SerializeField] string bossName;
    [SerializeField] GameObject bossHelthBar;
    string[] bossDialog = { "Hello Antero", "You have been a bad boy", "Now time to Die!!!" };
    DialogueManager dialogueManager;
    string dialogState = "";
    bool startDialog = false;
    GameObject InputInfo;
    float fadeInTexttime;
    Player player;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        player = FindObjectOfType<Player>();
        InputInfo = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        ProssesCharacterInteractions();
        ProssesInteraction();
    }
    void ProssesCharacterInteractions()
    {
        if (Input.GetButtonDown("Interact") && GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            player.GetComponent<Player>().enabled = false;
            if (!startDialog)
            {
                InputInfo.SetActive(false);
                dialogueManager.StartDialogue(bossName, bossDialog, dialogState);
                startDialog = true;
            }
            else
            {
                dialogState = dialogueManager.DisplayNextSentence(dialogState);
            }
        }
    }
    void ProssesInteraction()
    {
        if (dialogState == "default")
        {
            player.GetComponent<Player>().enabled = true;
            transform.parent.gameObject.GetComponent<BossDamage>().enabled = true;
            transform.parent.gameObject.GetComponent<BossPathing>().enabled = true;
            bossHelthBar.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enabled) return;

        if (!startDialog)
        {
            InputInfo.SetActive(true);
            fadeInTexttime += (0.5f * Time.deltaTime);
            if (fadeInTexttime <= 1) InputInfo.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, (byte)(fadeInTexttime * 255));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled) return;
        InputInfo.SetActive(false);
        fadeInTexttime = 0f;
    }
}
