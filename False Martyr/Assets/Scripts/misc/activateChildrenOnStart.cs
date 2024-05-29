using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateChildrenOnStart : MonoBehaviour
{
    //activate all children on start
    void Start()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
