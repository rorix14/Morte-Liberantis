using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelleAttack : MonoBehaviour
{
    [SerializeField] AudioClip[] melleAudioClips;
    [SerializeField] [Range(0, 1)] float melleVolume = 0.133f;
    float attackTimer = 0;
    float attackDoration = 0.2f;
    float nextAttck = 0;
    float attackCoolDown = 2f;
    Vector2 positionUp = new Vector2(0.05f, 0.785f);
    Vector2 positionAngleUp = new Vector2(0.79f, 0.561f);
    Vector2 positionForward = new Vector2(0.85f, -0.042f);
    Vector2 positionAngleDown = new Vector2(0.75f, -0.441f);
    public Vector2 PositionUp
    {
        get { return positionUp; }
    }
    public Vector2 PositionAngleUp
    {
        get { return positionAngleUp; }
    }
    public Vector2 PositionForward
    {
        get { return positionForward; }
    }
    public Vector2 PositionAngleDown
    {
        get { return positionAngleDown; }
    }
    public void DoMelleAttack(Vector3 rotation)
    {
        if (Input.GetButtonDown("Melle Attack") && Time.time > nextAttck)
        {
            gameObject.SetActive(true);
            transform.eulerAngles = rotation;
            attackTimer = attackDoration;
            transform.parent.gameObject.GetComponent<Player>().AttackDoration = 0.5f;
            transform.parent.gameObject.GetComponent<Player>().PlayerInputs = new string[1] { "melle" };
            FindObjectOfType<GameScession>().GetComponent<AudioSource>().PlayOneShot(melleAudioClips[Random.Range(0, melleAudioClips.Length)], melleVolume);
            nextAttck = Time.time + attackCoolDown;
        }
        if (Time.time < nextAttck)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator ManangeInvaderMelle()
    {
        gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(0.57f);

        gameObject.SetActive(false);
    }

    void ChangeShottingPosition(Vector2 position)
    {
        Vector2 newest = new Vector2(transform.position.x, transform.position.y);
        Vector2 positionToAdd = new Vector2(Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180), 1);
        //melleAttack.transform.eulerAngles = transform.eulerAngles;
        //position = (newest + position);
    }
}
