using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class InvadeMenu : MonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Transform contentTransform;
    void Start()
    {
        StartCoroutine(UpdateList());
    }

    private void RemoveButtons()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            GameObject toRemove = contentTransform.GetChild(i).gameObject;
            Destroy(toRemove);
        }
    }

    IEnumerator UpdateList()
    {
        while (true)
        {
            StartCoroutine(DBManager.GetDBInfo(DBManager.Url + "currently_palying", MakeButtons));
            yield return new WaitForSeconds(5f);
        }
    }

    void MakeButtons(string jsonString)
    {
        RemoveButtons();

        JsonData jsonvale = JsonMapper.ToObject(jsonString);
      
        foreach (var id in jsonvale["ids"])
        {
            var newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(contentTransform);
            newButton.GetComponent<InvaderMenuButton>().SetText(id.ToString());
        }
    }
    public void GoToMain()
    {
        SceneManager.LoadScene(0);
    }
}
