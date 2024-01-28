using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalScroll : MonoBehaviour
{
    [SerializeField] float scrollRate = 2f;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * scrollRate);
    }
}
