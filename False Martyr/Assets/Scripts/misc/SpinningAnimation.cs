using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningAnimation : MonoBehaviour
{
    //serialized values
    [SerializeField] private float spin_speed;
    [SerializeField] private bool z_rotation;
    [SerializeField] private bool x_rotation;
    [SerializeField] private bool y_rotation;

    void Update()
    {
        //rotate
        if (z_rotation)
        {
            transform.Rotate(new Vector3(0, 0, spin_speed * Time.deltaTime));
        }else if (x_rotation)
        {
            transform.Rotate(new Vector3(spin_speed * Time.deltaTime,0,0));
        } else
        {
            transform.Rotate(new Vector3(0, spin_speed * Time.deltaTime,0));
        }
    }
}
