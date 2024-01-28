using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] int nextScene;
    [SerializeField] int doorNumber;
    [SerializeField] GameObject exitPoint; 

    public int DoorNumber
    {
        get { return doorNumber; }
    }
    public Vector2 ExitPosition
    {
        get { return exitPoint.transform.position; }
    }
    public Vector3 ExitAngle
    {
        get { return exitPoint.transform.eulerAngles; }
    }
    private void OnTriggerStay2D(Collider2D collision)
     {
        //DoorManager.instance.LoadScene(DoorNumber, nextScene);
        FindObjectOfType<DoorManager>().GetComponent<DoorManager>().LoadScene(DoorNumber, nextScene);
     }
}
