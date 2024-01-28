using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shereder : MonoBehaviour
{
    GameScession gameScession;
    private void Start()
    {
        gameScession = GameObject.FindGameObjectWithTag("Game Scession").GetComponent<GameScession>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
          StartCoroutine(gameScession.ProcessPlayerDeath());
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
