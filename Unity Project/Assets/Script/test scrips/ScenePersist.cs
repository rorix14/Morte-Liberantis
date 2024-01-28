using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    static ScenePersist instance = null;
    int startingSceneIndex;

    void Start()
    {
        if (!instance)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (startingSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }
}

/* PREVIOUS SCENEPERSIST WORKED BUT NOT FOR THE CURRENT REQUREMENTS OF THE GAME
 * COULD ALSO USE THIS CODE INSTED IF YOU PUT ON THE LEVEL EXIT "Destroy(FindObjectOfType<ScenePersist>().gameObject);"
 * BEFORE LAODING THE NEXT LEVEL
public class ScenePersist : MonoBehaviour
{
    int startingSceneIndex;
    private void Awake()
    {
        int numScenePersist = FindObjectsOfType<ScenePersist>().Length;

        if (numScenePersist > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentSceneIndex != startingSceneIndex)
        {
            Destroy(gameObject);
        }
    }
}
*/
