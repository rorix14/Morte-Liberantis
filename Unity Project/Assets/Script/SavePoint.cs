using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int saveId;
    [SerializeField] AudioClip onSaveSound;
    [SerializeField] [Range(0, 1)] float volume = 0.75f;
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameScession;
    [SerializeField] GameObject saveEffect;
    [SerializeField] GameObject InputInfo;
    bool hasPlayer;
    SaveDataManager saveDataManager;
    string jsonContent;
    float fadeInTexttime;
    float saveCooldown;
    // GameScession GameScession;

    public int SaveId
    {
        get { return saveId; }
    }
    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (gameScession == null)
        {
            gameScession = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }
    private void Start()
    {
        saveDataManager = FindObjectOfType<SaveDataManager>();
    }
    private void Update()
    {
        SaveGame();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        hasPlayer = true;
        InputInfo.SetActive(true);
        fadeInTexttime += (0.5f * Time.deltaTime);
        if (fadeInTexttime <= 1) InputInfo.GetComponent<TextMeshPro>().color = new Color32(255, 255, 255, (byte)(fadeInTexttime * 255));
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        hasPlayer = false;
        InputInfo.SetActive(false);
        fadeInTexttime = 0f;
    }
    void SaveGame()
    {
        if (hasPlayer && Input.GetButtonDown("Interact") && Time.time >= saveCooldown)
        {
            saveCooldown = Time.time + onSaveSound.length;
            GameObject effect = Instantiate(saveEffect, (transform.position + new Vector3(0, 1, 0)), transform.rotation);
            Destroy(effect, 1f);
            DBManager.LastSavedScene = SceneManager.GetActiveScene().buildIndex;
            DBManager.SavePointId = saveId;
            DBManager.AmmoAmought = gameScession.GetComponent<GameScession>().getAmmo;
            gameScession.GetComponent<GameScession>().ProcessPlayerHealth(gameScession.GetComponent<GameScession>().MaxHealth);
            /*jsonContent = saveDataManager.SaveData(DBManager.getUserid.id, DBManager.UserName, DBManager.AmmoAmought, saveId,
                SceneManager.GetActiveScene().buildIndex, DBManager.CharactersIds, DBManager.UpgradesNames);*/

            StartCoroutine(DBManager.UpdatePlayerDataOnSavePoint(DBManager.Url + "save_point", saveDataManager.SaveData(DBManager.getUserid.id, DBManager.UserName, DBManager.AmmoAmought, saveId,
                SceneManager.GetActiveScene().buildIndex, DBManager.CharactersIds, DBManager.UpgradesNames)));
            //print(jsonContent);
            //AudioSource.PlayClipAtPoint(onSaveSound, Camera.main.transform.position, volume);
            gameScession.GetComponent<AudioSource>().PlayOneShot(onSaveSound, volume);
        }
    }
}
