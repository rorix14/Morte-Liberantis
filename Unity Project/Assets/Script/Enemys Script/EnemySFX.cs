using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy SFX")]
public class EnemySFX : ScriptableObject
{
    [Header("Hurt SFX")]
    [SerializeField] AudioClip[] hurtAudioClips;
    [SerializeField] [Range(0, 1)] float hurtVolume = 0.25f;

    [Header("Shoot SFX")]
    [SerializeField] AudioClip[] shootAudioClips;
    [SerializeField] [Range(0, 1)] float shootVolume = 0.133f;

    [Header("Jump SFX")]
    [SerializeField] AudioClip[] jumpAudioClips;
    [SerializeField] [Range(0, 1)] float jumpVolume = 0.08f;

    [Header("Expolsion SFX")]
    [SerializeField] AudioClip explosionAudioClip;
    [SerializeField] [Range(0, 1)] float explosionVolume = 0.133f;

    [Header("Destroyed SFX")]
    [SerializeField] AudioClip[] destroyedAudioClips;
    [SerializeField] [Range(0, 1)] float destroyedVolume = 0.3f;
    [SerializeField] GameObject destroyEffect;

    [Header("Boss Shoot SFX")]
    [SerializeField] AudioClip[] bossShootAudioClips;
    [SerializeField] [Range(0, 1)] float boosShootVolume = 0.3f;

    public AudioClip[] HurtAudioClips
    {
        get { return hurtAudioClips; }
    }

    public float HurtVolume
    {
        get { return hurtVolume; }
    }
    public AudioClip[] ShootAudioClips
    {
        get { return shootAudioClips; }
    }

    public float ShootVolume
    {
        get { return shootVolume; }
    }

    public AudioClip ExplosionAudioClip
    {
        get { return explosionAudioClip; }
    }

    public float ExplosionVolume
    {
        get { return explosionVolume; }
    }

    public AudioClip[] DestroyedAudioClips
    {
        get { return destroyedAudioClips; }
    }
    
    public float DestroyedVolume
    {
        get { return destroyedVolume; }
    }

    public GameObject DestroyEffect
    {
        get { return destroyEffect; }
    }

    public AudioClip[]JumpAudioClips
    {
        get { return jumpAudioClips; }
    }

    public float JumpVolume
    {
        get { return jumpVolume; }
    }

    public AudioClip[] BossShootAudioClips
    {
        get { return bossShootAudioClips; }
    }

    public float BoosShootVolume
    {
        get { return boosShootVolume; }
    }
    public IEnumerator ChangeColor(SpriteRenderer imageColor)
    {
        //Color32 startingColor = imageColor.color;
        //imageColor.color = new Color32(217, 15, 15, 255);
        imageColor.color = new Color32(255, 255, 255, 80);
        yield return new WaitForSeconds(0.2f);
        imageColor.color = new Color32(255, 255, 255, 255);
        //imageColor.color = startingColor;
    }

    public float VolumeMultiplier(Vector3 enemyPos)
    {
        float volumeMultiplier = 0;
        Player player = FindObjectOfType<Player>();
        float playerPosX = player.transform.position.x;
        float playerPosY = player.transform.position.y;

        float difX = Mathf.Abs(playerPosX - enemyPos.x);
        float difY = Mathf.Abs(playerPosY - enemyPos.y);

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
