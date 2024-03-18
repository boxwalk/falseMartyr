using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_rooms : MonoBehaviour
{
    // References
    private ReferenceController reference;
    private camera_controller cam;
    private room_controller room;

    //room movement offsets
    [SerializeField] private float bottom_door_offset;
    [SerializeField] private float top_door_offset;
    [SerializeField] private float left_door_offset;
    [SerializeField] private float right_door_offset;

    void Start()
    {
        //get references 
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        room = reference.RoomController;
    }

    //entering doors behaviour
    void Top_door()
    {
        cam.room_index = new Vector2Int(cam.room_index.x, cam.room_index.y+1);
        Vector3 new_pos = room.GetPositionFromGridIndex(cam.room_index);
        transform.position = new Vector3(new_pos.x, new_pos.y + top_door_offset, 0f);
    }
    void Bottom_door()
    {
        cam.room_index = new Vector2Int(cam.room_index.x, cam.room_index.y-1);
        Vector3 new_pos = room.GetPositionFromGridIndex(cam.room_index);
        transform.position = new Vector3(new_pos.x, new_pos.y + bottom_door_offset, 0f);
    }
    void Left_door()
    {
        cam.room_index = new Vector2Int(cam.room_index.x-1, cam.room_index.y);
        Vector3 new_pos = room.GetPositionFromGridIndex(cam.room_index);
        transform.position = new Vector3(new_pos.x + left_door_offset, new_pos.y, 0f);
    }
    void Right_door()
    {
        cam.room_index = new Vector2Int(cam.room_index.x+1, cam.room_index.y);
        Vector3 new_pos = room.GetPositionFromGridIndex(cam.room_index);
        transform.position = new Vector3(new_pos.x + right_door_offset, new_pos.y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision with door hitboxes
        if (collision.gameObject.tag == "TopDoor")
            Top_door();
        else if (collision.gameObject.tag == "BottomDoor")
            Bottom_door();
        else if (collision.gameObject.tag == "LeftDoor")
            Left_door();
        else if (collision.gameObject.tag == "RightDoor")
            Right_door();
    }
}
