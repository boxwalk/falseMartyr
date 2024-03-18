using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room_logic : MonoBehaviour
{
    //doors
    [SerializeField] private GameObject top_door;
    [SerializeField] private GameObject bottom_door;
    [SerializeField] private GameObject left_door;
    [SerializeField] private GameObject right_door;

    [SerializeField] private BoxCollider2D top_door_hitbox;
    [SerializeField] private BoxCollider2D bottom_door_hitbox;
    [SerializeField] private BoxCollider2D left_door_hitbox;
    [SerializeField] private BoxCollider2D right_door_hitbox;

    //door index property (get set is shorthand to set up property)
    public Vector2Int room_index { get; set; }

    //room type property
    public string room_type { get; set; }

    //other values (set and read from other scripts)
    public int enemy_count = 1;

    //room rewards 
    public List<string> rewards = new();

    //open doors
    public void open_door(Vector2Int direction)
    {
        if(direction == Vector2Int.up)
        {
            //open top door
            top_door.SetActive(true);
            top_door_hitbox.enabled = false;
        }
        else if (direction == Vector2Int.down)
        {
            //open bottom door
            bottom_door.SetActive(true);
            bottom_door_hitbox.enabled = false;
        }
        else if (direction == Vector2Int.left)
        {
            //open left door
            left_door.SetActive(true);
            left_door_hitbox.enabled = false;
        }
        else if (direction == Vector2Int.right)
        {
            //open right door
            right_door.SetActive(true);
            right_door_hitbox.enabled = false;
        }
    }

    //close existing doors for combat
    public void close_door(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            //close top door
            top_door.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (direction == Vector2Int.down)
        {
            //close bottom door
            bottom_door.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (direction == Vector2Int.left)
        {
            //close left door
            left_door.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (direction == Vector2Int.right)
        {
            //close right door
            right_door.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    //open existing doors after combat
    public void reopen_door(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            //close top door
            top_door.transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (direction == Vector2Int.down)
        {
            //close bottom door
            bottom_door.transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (direction == Vector2Int.left)
        {
            //close left door
            left_door.transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (direction == Vector2Int.right)
        {
            //close right door
            right_door.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
