using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameplayController : MonoBehaviour
{
    //references
    private ReferenceController reference;
    private camera_controller cam;
    private room_controller room_controller;

    //rooms
    private Vector2Int  previous_room = new Vector2Int(0,0);

    //enemy room values
    [HideInInspector] public bool enemy_room_clear = true;

    //prefabs
    [SerializeField] private GameObject heart_pickup;
    [SerializeField] private GameObject spirit_heart_pickup;
    [SerializeField] private GameObject coin_pickup;

    //layermasks
    [SerializeField] private LayerMask player_mask;

    //reward spawning
    private List<int> heart_spawns = new List<int> { 1, 1, 1, 2 };
    private List<int> coin_spawns = new List<int> { 1, 1, 2, 2 ,2 ,3 ,3 ,3 ,4};

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        room_controller = reference.RoomController;
    }

    void Update()
    {
        room_logic room_script = room_controller.get_room_script_at_index(cam.room_index); //reference script of current room

        if (previous_room != cam.room_index) //new room entered
        { 
            GameObject new_room = room_script.gameObject; //reference to room object

            if ((room_controller.enemy_rooms.Contains(new_room)) && (room_script.enemy_count > 0)) //enemy room
            {
                if (new_room.transform.GetChild(1).GetChild(0).gameObject.activeInHierarchy) //close doors
                    room_script.close_door(Vector2Int.left); //close left door
                if (new_room.transform.GetChild(1).GetChild(1).gameObject.activeInHierarchy) 
                    room_script.close_door(Vector2Int.right); //close right door
                if (new_room.transform.GetChild(1).GetChild(2).gameObject.activeInHierarchy) 
                    room_script.close_door(Vector2Int.up); //close top door
                if (new_room.transform.GetChild(1).GetChild(3).gameObject.activeInHierarchy) 
                    room_script.close_door(Vector2Int.down); //close bottom door

                enemy_room_clear = false; //set room clear to false
            }
        }

        if(!enemy_room_clear && room_script.enemy_count <= 0) //check for combat room clear
        {
            enemy_room_clear = true; //room clear

            GameObject new_room = room_script.gameObject; //reference to room object

            StartCoroutine(clear_room(new_room,room_script)); //open doors

            spawn_room_rewards(room_script.rewards); //spawn room rewards
        }

        //set previous room
        previous_room = cam.room_index;
    }

    private IEnumerator clear_room(GameObject new_room, room_logic room_script)
    {
        new_room.GetComponent<Animator>().SetTrigger("open_doors");
        yield return new WaitForSeconds(0.33f);

        if (new_room.transform.GetChild(1).GetChild(0).gameObject.activeInHierarchy) //open doors
            room_script.reopen_door(Vector2Int.left); //open left door
        if (new_room.transform.GetChild(1).GetChild(1).gameObject.activeInHierarchy)
            room_script.reopen_door(Vector2Int.right); //open right door
        if (new_room.transform.GetChild(1).GetChild(2).gameObject.activeInHierarchy)
            room_script.reopen_door(Vector2Int.up); //open top door
        if (new_room.transform.GetChild(1).GetChild(3).gameObject.activeInHierarchy)
            room_script.reopen_door(Vector2Int.down); //open bottom door
    }

    private void spawn_room_rewards(List<string> rewards)
    {
        //spawn room rewards
        foreach(string reward in rewards)
        {
            if(reward == "heart") //spawn heart
            {
                spawn_pickup(heart_pickup, heart_spawns[Random.Range(0,heart_spawns.Count)]);
            }else if(reward == "spirit") //spawn spirit heart
            {
                spawn_pickup(spirit_heart_pickup, 1);
            }else if(reward == "coin") //spawn coin
            {
                spawn_pickup(coin_pickup, coin_spawns[Random.Range(0, coin_spawns.Count)]);
            }
        }
    }

    private void spawn_pickup(GameObject pickup, int count)
    {
        for(int i = 0; i < count; i++)
        {
            Vector3 room_centre = room_controller.GetPositionFromGridIndex(cam.room_index);
            Vector2 spawn_pos = new Vector2(Random.Range(-7.63f, 7.7f) + room_centre.x, Random.Range(-3.94f, 3.91f) + room_centre.y); //pick spawn location
            while (Physics2D.OverlapCircle(spawn_pos, 0.5f, player_mask))
            {
                spawn_pos = new Vector2(Random.Range(-7.63f, 7.7f) + room_centre.x, Random.Range(-3.94f, 3.91f) + room_centre.y); //pick spawn location
            }
            Instantiate(pickup, spawn_pos, Quaternion.identity); //spawn pickup
        }
    }
}



