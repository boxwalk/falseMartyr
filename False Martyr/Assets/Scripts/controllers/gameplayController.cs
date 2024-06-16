using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameplayController : MonoBehaviour
{
    //references
    private ReferenceController reference;
    private camera_controller cam;
    private room_controller room_controller;
    private itemController itemController;

    //rooms
    private Vector2Int  previous_room = new Vector2Int(0,0);

    //enemy room values
    [HideInInspector] public bool enemy_room_clear = true;
    [HideInInspector] public string currentRoomType = "none";

    //prefabs
    [SerializeField] private GameObject heart_pickup;
    [SerializeField] private GameObject spirit_heart_pickup;
    [SerializeField] private GameObject coin_pickup;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject itemPedestal;

    //layermasksp
    [SerializeField] private LayerMask player_mask;
    [SerializeField] private LayerMask reward_mask;

    //reward spawning
    private List<int> heart_spawns = new List<int> { 1, 1, 1, 2 };
    private List<int> coin_spawns = new List<int> { 1, 1, 2, 2 ,2 ,3 ,3 ,3 ,4};

    //serialized values
    [SerializeField] private Vector2 boss_end_box;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        room_controller = reference.RoomController;
        itemController = reference.ItemController;
    }

    void Update()
    {
        room_logic room_script = room_controller.get_room_script_at_index(cam.room_index); //reference script of current room

        //get values from room script
        currentRoomType = room_script.room_type;

        if (previous_room != cam.room_index) //new room entered
        { 
            GameObject new_room = room_script.gameObject; //reference to room object

            if ((room_controller.enemy_rooms.Contains(new_room) || new_room == room_controller.bossRoom )&& (room_script.enemy_count > 0)) //enemy room
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

            if(room_script.room_type == "boss")
                StartCoroutine(clear_boss_room(room_script)); //clear boss room
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
            while (Physics2D.OverlapCircle(spawn_pos, 0.5f, reward_mask))
            {
                spawn_pos = new Vector2(Random.Range(-7.63f, 7.7f) + room_centre.x, Random.Range(-3.94f, 3.91f) + room_centre.y); //pick spawn location
            }
            Instantiate(pickup, spawn_pos, Quaternion.identity); //spawn pickup
        }
    }

    private IEnumerator clear_boss_room(room_logic room_script) //boss room clear logic
    {
        Vector3 room_centre = room_controller.GetPositionFromGridIndex(room_script.room_index);
        while (Physics2D.OverlapBox(room_centre, boss_end_box, 0, player_mask)) //wait for player to leave center of room
        {
            yield return null;
        }
        Instantiate(exit,room_centre,Quaternion.identity); //spawn exit

        //spawn item
        int item = itemController.getItemFromItemPool(); //pick item
        float pedestal_offset = Random.Range(0, 2);
        pedestal_offset = pedestal_offset == 0 ? 2.21f :-2.21f;
        Instantiate(itemPedestal, new Vector2(room_centre.x + pedestal_offset, room_centre.y), Quaternion.identity); //spawn pedestal
        GameObject instantiated_item = Instantiate(itemController.item_prefab, new Vector3(room_centre.x + pedestal_offset, room_centre.y + itemController.item_library[item].item_y_offset, room_centre.z), Quaternion.identity); //spawn items
        instantiated_item.GetComponent<SpriteRenderer>().sprite = itemController.item_library[item].item_sprite; //set item sprite
        instantiated_item.GetComponent<itemLogic>().item_id = item; //set id correctly
        instantiated_item.GetComponent<Animator>().SetTrigger("spawn"); // play spawn animation
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, boss_end_box);
    }
    */

    public void nextFloor() //exit reached
    {
        SceneManager.LoadScene(2); //load end screen
    }
}



