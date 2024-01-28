using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damege = 100;
    /*[SerializeField] GameObject fieredEffect;
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;
    Player player;*/

   /* private void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();

        ProcessFieredShoot();
    }
    private void Update()
    {
       // fieredEffect.GetComponent<Rigidbody2D>().velocity = (player.GetComponent<Rigidbody2D>().velocity * 5000f);
    }*/
    public int Damage
    {
        get { return damege; }
    }
    /*public int GetDamage()
    {
        return damege;
    }*/
   /* public void Hit()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ProcessDestroy());
    }

    IEnumerator ProcessDestroy()
    {
        myAnimator.SetTrigger("Colided");
        myRigidbody2D.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(0.4f);

        Hit();
    }

    public void ProcessFieredShoot()
    {
        GameObject projectile = Instantiate(fieredEffect, player.transform.position,transform.rotation) as GameObject;
        fieredEffect.GetComponent<Rigidbody2D>().velocity = (player.GetComponent<Rigidbody2D>().velocity * 5000f);
        Destroy(projectile, 0.2f);
    }*/
}