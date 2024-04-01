using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applySinWave : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float magnitude;
    [SerializeField] private float root_y;
    void Update()
    {
        float y_offset = Mathf.Sin(Time.time * speed) * magnitude;
        transform.localPosition = new Vector2(transform.localPosition.x, root_y + y_offset);
    }
}
