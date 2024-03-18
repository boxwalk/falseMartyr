using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class enemyAbstract : MonoBehaviour
{
    //variables to inherit
    protected Vector2Int room_index;
    protected bool enemy_activated = false;

    //references
    protected ReferenceController reference;
    protected camera_controller cam;
    protected enemyHealth enemyHealth;

    public void start_logic()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        enemyHealth = GetComponent<enemyHealth>();
    }

    public void update_logic()
    {
        room_index = enemyHealth.room_index;
        if (!enemy_activated)
            enemy_activated = cam.room_index == room_index; //check whether to activate enemy
    }
}
