using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float delay = 1f;
    [SerializeField] float slowMo = 0.2f;
    void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(WaitForLoad());
    }

    IEnumerator WaitForLoad()
    {
        Time.timeScale = slowMo;
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1f;
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
