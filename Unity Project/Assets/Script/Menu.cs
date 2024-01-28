using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Button registerBtn;
    [SerializeField] Button loginBtn;
    [SerializeField] Button playBtn;
    [SerializeField] Button logOutBtn;
    [SerializeField] Button invadeBtn;
    [SerializeField] Text playerDisplay;
    private void Start()
    {
        manageMenuUI();
    }

    private void Update()
    {
        manageMenuUI();
    }
    public void StartFirstLevel()
    {
        //StartCoroutine(DBManager.getDailyQuestStatus());
        DBManager.isInvader = false;
        if (!DBManager.LastSavedScene.HasValue)
        {

            SceneManager.LoadScene(7);
        }
        else
        {
            SceneManager.LoadScene(DBManager.LastSavedScene.Value);
        }
    }
    public void GotoBegining()
    {
        SceneManager.LoadScene(0);
    }
    public void GoToRegister()
    {
        SceneManager.LoadScene("Register Menu");
    }
    public void GoToLogIn()
    {
        SceneManager.LoadScene("Log In Menu");
    }
    public void GoToOffLine()
    {
        SceneManager.LoadScene("Play Off Line Menu");
    }
    public void CallLogOut()
    {
        DBManager.LogOut();
    }
    public void Invade()
    {
        SceneManager.LoadScene("Invade Menu");
    }

    private void manageMenuUI()
    {
        try
        {
            if (DBManager.LoggedIn)
            {
                playerDisplay.text = "User: " + DBManager.UserName;
            }
            else
            {
                playerDisplay.text = "No user logged in";
            }
            registerBtn.interactable = !DBManager.LoggedIn;
            loginBtn.interactable = !DBManager.LoggedIn;
            playBtn.interactable = DBManager.LoggedIn;
            invadeBtn.interactable = DBManager.LoggedIn;
            logOutBtn.interactable = DBManager.LoggedIn;
        }
        catch (Exception e)
        {
            Debug.Log(e + "  Exception caught.");
        }

    }
}
