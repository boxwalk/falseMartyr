using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapController : MonoBehaviour
{
    //prefabs
    [SerializeField] private GameObject minimap_room;

    //sprites
    [SerializeField] private Sprite active_room;
    [SerializeField] private Sprite discovered_room;
    [SerializeField] private Sprite undiscovered_room;

    //transforms and gameobjects
    [SerializeField] private Transform minimap_parent;

    //values
    private bool former_minimap_active = false;
    public float room_x;
    public float room_y;
    public List<Vector2Int> discovered_rooms = new List<Vector2Int>() {new Vector2Int(25, 25),new Vector2Int(26, 25),new Vector2Int(24, 25),new Vector2Int(25, 24),new Vector2Int(25, 26)}; //initialize discovered list
    public List<Vector2Int> entered_rooms = new List<Vector2Int>() { new Vector2Int(25, 25) }; //initialize entered list
    public Vector2Int room_index;
    public float scale = 1f;

    //components
    private Animator anim;

    //references
    private ReferenceController reference;
    private room_controller roomController;
    private camera_controller playerIndex;
    private gameController gameController;

    void Start()
    {
        //get components
        anim = minimap_parent.gameObject.GetComponent<Animator>();
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        roomController = reference.RoomController;
        playerIndex = reference.CameraController;
        gameController = reference.GameController;
    }

    void Update()
    {
        minimap_showing();
        update_lists();
        check_for_shrink();
    }

    void minimap_showing() //only shows minimap if tab is pressed
    {
        bool tabpressed = false;

        if (gameController.fullGameStart)
            tabpressed = Input.GetKey(KeyCode.Tab); //get input

        if(tabpressed && !former_minimap_active && anim.GetCurrentAnimatorStateInfo(0).IsName("minimap_idle")) //apply to animator
        {
            anim.SetTrigger("enter");
        }
        if (!tabpressed && anim.GetCurrentAnimatorStateInfo(0).IsName("minimap_in_idle"))
        {
            anim.SetTrigger("exit");
        }

        former_minimap_active = tabpressed; //set former value
    }

    public void instantiate_minimap()
    {
        foreach( GameObject room in roomController.rooms_list)
        {
            GameObject obj = Instantiate(minimap_room, minimap_parent.GetChild(0));
            minimapRoom script = obj.GetComponent<minimapRoom>();
            room_logic room_script = room.GetComponent<room_logic>();
            script.room_index = room_script.room_index;
            script.room_type = room_script.room_type;
        }
    }

    void update_lists()
    {
        room_index = playerIndex.room_index; //get current room index

        if (!entered_rooms.Contains(room_index)) 
        {
            entered_rooms.Add(room_index); //add to entered rooms
        }

        if (!discovered_rooms.Contains(room_index))
        {
            discovered_rooms.Add(room_index); //add to discovered rooms
        }

        //add left right up down rooms to discovered
        if (!discovered_rooms.Contains(new Vector2Int(room_index.x - 1,room_index.y))) //left
        {
            discovered_rooms.Add(new Vector2Int(room_index.x -1, room_index.y));
        }
        if (!discovered_rooms.Contains(new Vector2Int(room_index.x + 1, room_index.y))) //right
        {
            discovered_rooms.Add(new Vector2Int(room_index.x + 1, room_index.y));
        }
        if (!discovered_rooms.Contains(new Vector2Int(room_index.x, room_index.y + 1))) //up
        {
            discovered_rooms.Add(new Vector2Int(room_index.x, room_index.y + 1));
        }
        if (!discovered_rooms.Contains(new Vector2Int(room_index.x, room_index.y - 1))) //down
        {
            discovered_rooms.Add(new Vector2Int(room_index.x, room_index.y - 1));
        }
    }

    void check_for_shrink()
    {
        bool limit_broken = false;
        foreach(Vector2Int index in discovered_rooms)
        {
            if (index.x < 20 || index.x > 30 || index.y < 21 || index.y > 29) //cehck for limit breaks
                limit_broken = true;
        }
        if (limit_broken)
            scale = 0.5f;
        else
            scale = 1;
    }
}
