using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] int coinPoints = 10;
    [SerializeField] AudioClip coinPickUpSFX;
    [SerializeField] [Range(0, 1)] float volume = 0.75f;
    bool hasColected = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        //hasColected = true;
        FindObjectOfType<GameScession>().GetComponent<AudioSource>().PlayOneShot(coinPickUpSFX, volume);
        //AudioSource.PlayClipAtPoint(coinPickUpSFX, Camera.main.transform.position, volume);
        StartCoroutine(DoEffects());
        //Destroy(gameObject);
    }
  
    IEnumerator DoEffects()
    {
        if (!hasColected)
        {
            FindObjectOfType<GameScession>().AddToScore(coinPoints);
            hasColected = true;
        }
        yield return null;
        Destroy(gameObject);
    }
}
