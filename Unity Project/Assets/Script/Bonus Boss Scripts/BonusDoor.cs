using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;

public class BonusDoor : MonoBehaviour
{
    [SerializeField] Light2D portalLight;
    private bool hasCompleatedTextAdventure;
    float gain;
    float lightInensitiy;
    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
        portalLight.intensity = 0;
    }


    IEnumerator Start()
    {
        yield return new WaitUntil(() => DBManager.CharactersArry != null);

        foreach (var character in DBManager.CharactersArry)
        {
            StartCoroutine(DBManager.GetDBInfo(DBManager.Url + "user/" + DBManager.getUserid.id + "/characters/" + character.id,
                jsonString =>
                {
                    JsonData jsonvale = JsonMapper.ToObject(jsonString);
                    if (jsonvale["textAdventureDone"].ToString() == "1")
                    {
                        hasCompleatedTextAdventure = true;
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
                    }
                }));
        }
    }
    private void Update()
    {
        if (hasCompleatedTextAdventure)
        {
            gain += (0.3f * Time.deltaTime);
            if (gain <= 1) GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)((gain) * 255));

            lightInensitiy += (0.4f * Time.deltaTime);
            if (lightInensitiy <= 1.5f) portalLight.intensity = lightInensitiy;
        }

        CheatEntrance();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasCompleatedTextAdventure)
        {
            SceneManager.LoadScene("test 6");
        }
    }

    private void CheatEntrance()
    {
        if (Input.GetKeyDown("escape")) hasCompleatedTextAdventure = true;
    }
}
