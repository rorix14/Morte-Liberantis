using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InvaderMenuButton : MonoBehaviour
{
    [SerializeField] Text buttonText;
    private string id;
    public void SetText(string id)
    {
        this.id = id;
        buttonText.text = "Player id: " + id;
    }
    public void InvadePlayer()
    {
        StartCoroutine(DBManager.updatePlayerPlayingStatus(DBManager.Url + "currently_palying/" + id + "/" + 0, ProcessResults));
    }

    private void ProcessResults(string result)
    {
        if (result == "1")
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("invaderId", DBManager.getUserid.id);
            StartCoroutine(DBManager.UpdatePlayerInvaderId(DBManager.Url + "update_invader_id", form, SuccessfullInvasion));
        }
        else
        {
            buttonText.text = "Player no loger playing";
        }
    }

    void SuccessfullInvasion(string result)
    {
        DBManager.isInvader = true;
        DBManager.InvadedPlayerId = id;
        SceneManager.LoadScene(7);
    }
}
