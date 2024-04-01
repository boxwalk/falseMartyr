using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollisionWithTag : MonoBehaviour
{
    public string _tag;
    [HideInInspector] public bool isTouching = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_tag))
            isTouching = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_tag))
            isTouching = false;
    }
}
