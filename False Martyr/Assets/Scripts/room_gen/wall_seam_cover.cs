using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_seam_cover : MonoBehaviour
{
    [SerializeField] private GameObject top_door;
    private SpriteRenderer rend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        rend.enabled = !top_door.activeInHierarchy;
    }
}
