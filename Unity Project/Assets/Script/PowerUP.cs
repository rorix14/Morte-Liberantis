using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUP : MonoBehaviour
{
    [SerializeField] string upgradeName;
    [SerializeField] float delay = 5f;
    [SerializeField] float slowMo = 0f;
    [SerializeField] AudioClip collectedSound;
    [SerializeField] [Range(0, 1)] float volume = 0.75f;
    bool hasColected = false;
    GameObject gameScession;
    //Player player;

    private void Awake()
    {
        // if (DBManager.DoubleJumpPWUp) Destroy(gameObject);
        foreach (string item in DBManager.UpgradesNames)
        {
            if (item.Equals(upgradeName))
            {
                DBManager.InitializePlayerUpgrades();
                Destroy(gameObject);
            }
        }
        if (gameScession == null)
        {
            gameScession = GameObject.FindGameObjectWithTag("Game Scession");
        }
        //player = FindObjectOfType<Player>();

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //player.GetComponent<AudioSource>().PlayOneShot(collectedSound, volume);
        gameScession.GetComponent<AudioSource>().PlayOneShot(collectedSound, volume);
        //AudioSource.PlayClipAtPoint(collectedSound, Camera.main.transform.position, volume);
        StartCoroutine(DoEffects());
    }
    IEnumerator DoEffects()
    {
        if (!hasColected)
        {
            DBManager.AddToUpgardesList = upgradeName;
            hasColected = true;
        }

        Time.timeScale = slowMo;
        //DBManager.DoubleJumpPWUp = true;
        DBManager.InitializePlayerUpgrades();
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
