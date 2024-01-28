using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] GameObject fieredEffect;
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;
    BoxCollider2D boxCollider2D;
    [SerializeField] AudioClip[] inpactAudioClips;
    [SerializeField] [Range(0, 1)] float volume = 0.06f;
    [HideInInspector]
    [SerializeField] GameObject gameScessionObj;
    Vector2 velocity;
    Color32 v = new Color32(255, 255, 255, 255);
    //Player player;
    //BoxCollider2D collider2D;
    private void Awake()
    {
        if (gameScessionObj == null)
        {
            gameScessionObj = GameObject.FindGameObjectWithTag("Game Scession");
        }
    }
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        velocity = myRigidbody2D.velocity;
       // player = FindObjectOfType<Player>();
       //collider2D = GetComponent<BoxCollider2D>();
       // processfieredshoot();
    }
    private void Update()
    {
        if(!myRigidbody2D.velocity.Equals(velocity))
        {
            //print("do I have a diferent velocity?");
            myRigidbody2D.velocity = velocity;
        }
    }
    public void Hit()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       // collider2D.enabled = false;
        StartCoroutine(ProcessDestroy());
    }

    IEnumerator ProcessDestroy()
    {
        myAnimator.SetTrigger("Colided");
        //myRigidbody2D.velocity = new Vector2(0, 0);
        velocity = new Vector2(0, 0);
        //boxCollider2D.enabled = false;
        gameScessionObj.GetComponent<AudioSource>().PlayOneShot(inpactAudioClips[Random.Range(0, inpactAudioClips.Length)], volume * VolumeMultiplier());
        yield return new WaitForSeconds(0.4f);
        Hit();
    }

    public void ProcessFieredShoot(Vector3 position, Vector3 rotation, bool chageColor = false)
    {
        GameObject projectile = Instantiate(fieredEffect, position, Quaternion.Euler(rotation)) as GameObject;
        if(chageColor)
        {
            projectile.GetComponent<SpriteRenderer>().color = new Color32(14, 255, 93, 255);
        }
        //projectile.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
        Destroy(projectile, 0.2f);
    }

    public void MantainVelocity(Vector2 velocity)
    {
        this.velocity = velocity;
    }

    private float VolumeMultiplier()
    {
        float volumeMultiplier = 0;
        Player player = FindObjectOfType<Player>();
        float playerPosX = player.transform.position.x;
        float playerPosY = player.transform.position.y;

        float difX = Mathf.Abs(transform.position.x - playerPosX);
        float difY = Mathf.Abs(transform.position.y - playerPosY);

        if (difX <= 10 && difY <= 10)
        {
            volumeMultiplier = 1f;
            return volumeMultiplier;
        }
        else if (difX <= 15 && difY <= 15)
        {
            volumeMultiplier = 0.7f;
            return volumeMultiplier;
        }
        else if (difX <= 20 && difY <= 20)
        {
            volumeMultiplier = 0.4f;
            return volumeMultiplier;
        }
        else if (difX <= 25 && difY <= 25)
        {
            volumeMultiplier = 0.2f;
            return volumeMultiplier;
        }
        else if (difX <= 30 && difY <= 30)
        {
            volumeMultiplier = 0.05f;
            return volumeMultiplier;
        }
        else { return volumeMultiplier; }
    }
}

