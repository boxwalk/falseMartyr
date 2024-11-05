using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room_controller : MonoBehaviour
{
    //room prefab
    [SerializeField] private GameObject room_prefab;
    [SerializeField] private GameObject floor2Room;

    //room dimensions 
    [SerializeField] private int room_width;
    [SerializeField] private int room_height;

    //entire floor dimensions 
    public int grid_size_x;
    public int grid_size_y;

    //room count
    [SerializeField] private int max_rooms;
    [SerializeField] private int min_rooms;

    //rooms list
    [HideInInspector] public List<GameObject> rooms_list = new List<GameObject>();

    //2d grid array
    private int[,] room_grid;

    //room total
    private int room_count;

    //queue of rooms (queues are lists ordered by time added)
    private Queue<Vector2Int> room_queue = new Queue<Vector2Int>();

    //generation complete boolean
    private bool generation_complete = false;
    private bool generation_stageone_successfull = false;
    [HideInInspector] public bool full_gen_complete = false;

    //end rooms list
    [HideInInspector] public List<GameObject> end_rooms = new List<GameObject>();
    [SerializeField] private int min_end_rooms;
    [SerializeField] private int max_end_rooms;

    //room type lists
    [HideInInspector] public List<GameObject> enemy_rooms = new List<GameObject>();
    [HideInInspector] public List<GameObject> item_rooms_list = new List<GameObject>();
    [SerializeField] private int item_rooms;
    public GameObject bossRoom; 

    //references to controllers
    private ReferenceController referenceController;
    private enemyGenController enemyGenController;
    private itemController itemController;
    private shopController shopController;
    private minimapController minimapController;
    private statController stats;
    private roomConfigController configController;
    private gameController gameController;

    //prefabs
    public GameObject item_pedestal;
    [SerializeField] private GameObject shop_item;
    [SerializeField] private GameObject shopkeep;

    //shop values
    private GameObject shop_room;
    [SerializeField] private List<Vector2> shop_slot_pos;


    void Start()
    {
        //initialize variables
        room_grid = new int[grid_size_x, grid_size_y];
        room_queue = new Queue<Vector2Int>();

        //get references 
        referenceController = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        enemyGenController = referenceController.enemyGenController;
        itemController = referenceController.ItemController;
        shopController = referenceController.ShopController;
        minimapController = referenceController.MinimapController;
        stats = referenceController.StatController;
        configController = referenceController.RoomConfigController;
        gameController = referenceController.GameController;

        //pick floor
        if(gameController.floor == 2) //floor 2
        {
            room_prefab = floor2Room;
        }

        //begin generation
        Vector2Int initial_room_index = new Vector2Int(grid_size_x / 2, grid_size_y / 2);
        StartRoomGenFromRoom(initial_room_index);

    }

    void Update()
    {
        if (!generation_stageone_successfull)
        {
            //update logic
            if (room_queue.Count > 0 && room_count < max_rooms && !generation_complete)
            {
                Vector2Int room_index = room_queue.Dequeue(); //use the last generated room as a stem
                int grid_x = room_index.x;
                int grid_y = room_index.y;

                if (grid_x > 0 && room_grid[grid_x - 1, grid_y] == 0)
                {
                    //No neighbor to the left
                    generate_room(new Vector2Int(grid_x - 1, grid_y)); //left room
                }
                if (grid_x < grid_size_x - 1 && room_grid[grid_x + 1, grid_y] == 0)
                {
                    //No neighbor to the right
                    generate_room(new Vector2Int(grid_x + 1, grid_y)); //right room
                }
                if (grid_y > 0 && room_grid[grid_x, grid_y - 1] == 0)
                {
                    //No neighbor below
                    generate_room(new Vector2Int(grid_x, grid_y - 1)); //bottom room
                }
                if (grid_y < grid_size_y - 1 && room_grid[grid_x, grid_y + 1] == 0)
                {
                    //No neighbor above
                    generate_room(new Vector2Int(grid_x, grid_y + 1)); //upper room
                }

            }
            else if (room_count < min_rooms)
            {
                //generation failed
                Debug.Log($"generation failed with {room_count} rooms.");
                regenerate_rooms();

            }
            else if (!generation_complete)
            {
                //determine end rooms
                determine_end_rooms();
                if(end_rooms.Count < min_end_rooms || end_rooms.Count > max_end_rooms) //check if there are enough end rooms
                {
                    //generation failed
                    Debug.Log($"generation failed with {end_rooms.Count} end rooms.");
                    regenerate_rooms();
                }
                else //generation successful
                {
                    generation_complete = true; //generation completed
                    Debug.Log($"generation complete with {room_count} rooms.");
                    generation_stageone_successfull = true;
                    generation_stage_two();
                }
            }
        }
    }

    //begins the generation from a certain index
    private void StartRoomGenFromRoom(Vector2Int room_index)
    {
        room_queue.Enqueue(room_index); //adds index to the queue
        int x = room_index.x;
        int y = room_index.y;
        room_grid[x, y] = 1; //creates a room in the array
        room_count++; //increments the room count
        var inital_room = Instantiate(room_prefab, GetPositionFromGridIndex(room_index), Quaternion.identity); //instantiate the room prefab
        inital_room.name = $"room-{ room_count}"; //$ strings are the equivalent of f-strings in python
        inital_room.GetComponent<room_logic>().room_index = room_index;
        rooms_list.Add(inital_room); //adds the prefab to the list of rooms
    }

    //generates a new room
    private bool generate_room(Vector2Int room_index) //returns false if failed, true if successfull
    {
        int x = room_index.x;
        int y = room_index.y;
        bool chance = Random.value < 0.5f; // random chance (random.value is 0 to 1)

        int distance = Mathf.Abs((grid_size_x / 2) - x) + Mathf.Abs((grid_size_y / 2) - y); // distance to check for higher probability
        if (distance == 1) //next to inital room
            chance = Random.value < 0.7f; //higher chance to spawn
        else if (distance == 2) //near initial room
            chance = Random.value < 0.75f; //higher chance to spawn

        if (room_count >= max_rooms) // generation fails if exceeding the maximum rooms
            return false;

        if (!chance && room_index != Vector2Int.zero) //random room generation 
            return false;

        if (count_adjancent_rooms(room_index) > 1) //checks if there are adjacent rooms to prevent clumping rooms. REMOVE for clumpy room gen KEEP for snakey dungeon feel
            return false;

        room_queue.Enqueue(room_index); //adds index to the queue
        room_grid[x, y] = 1; //creates a room in the array
        room_count++; //increments the room count

        var new_room = Instantiate(room_prefab, GetPositionFromGridIndex(room_index), Quaternion.identity); //instantiate the room prefab
        new_room.name = $"room-{ room_count}"; //$ strings are the equivalent of f-strings in python
        new_room.GetComponent<room_logic>().room_index = room_index;
        rooms_list.Add(new_room); //adds the prefab to the list of rooms
        open_doors(new_room, x, y); //open doors for new room
        return true; //room generated successfully
    }

    //function to return transform of room from index on grid
    public Vector3 GetPositionFromGridIndex(Vector2Int grid_index)
    {
        int grid_x = grid_index.x;
        int grid_y = grid_index.y;
        return new Vector3(room_width * (grid_x - grid_size_x / 2), room_height * (grid_y - grid_size_y / 2)); //calculates transform from index
    }

    //function that counts adjacent rooms
    private int count_adjancent_rooms(Vector2Int room_index)
    {
        int room_x = room_index.x;
        int room_y = room_index.y;
        int count = 0;

        if (room_x > 0 && room_grid[room_x - 1, room_y] != 0) count++; //left neighbour
        if (room_x < grid_size_x - 1 && room_grid[room_x + 1, room_y] != 0) count++; //right neighbour
        if (room_y > 0 && room_grid[room_x, room_y - 1] != 0) count++; //downwards neighbour
        if (room_y < grid_size_y - 1 && room_grid[room_x, room_y + 1] != 0) count++; //upwards neighbour

        return count;
    }

    //DOOR OPENING BABY!!!
    void open_doors(GameObject room, int x, int y)
    {
        room_logic room_script = room.GetComponent<room_logic>();

        //get scripts of neighbours
        room_logic left_room_script = get_room_script_at_index(new Vector2Int(x - 1, y));
        room_logic right_room_script = get_room_script_at_index(new Vector2Int(x + 1, y));
        room_logic up_room_script = get_room_script_at_index(new Vector2Int(x, y + 1));
        room_logic down_room_script = get_room_script_at_index(new Vector2Int(x, y - 1));

        //determine which doors to open
        if (x > 0 && room_grid[x - 1, y] != 0)
        {
            //left neighbour
            room_script.open_door(Vector2Int.left);
            left_room_script.open_door(Vector2Int.right);
        }
        if (x < grid_size_x - 1 && room_grid[x + 1, y] != 0)
        {
            //right neighbour
            room_script.open_door(Vector2Int.right);
            right_room_script.open_door(Vector2Int.left);
        }
        if (y > 0 && room_grid[x, y - 1] != 0)
        {
            //down neighbour
            room_script.open_door(Vector2Int.down);
            down_room_script.open_door(Vector2Int.up);
        }
        if (y < grid_size_y - 1 && room_grid[x, y + 1] != 0)
        {
            //up neighbour
            room_script.open_door(Vector2Int.up);
            up_room_script.open_door(Vector2Int.down);
        }
    }

    //returns a room script from a given index
    public room_logic get_room_script_at_index(Vector2Int index)
    {
        GameObject room_object = rooms_list.Find(r => r.GetComponent<room_logic>().room_index == index); //lambda expression, learn more about this later but here it finds the room object with the correct index from the list
        if (room_object != null)
            return room_object.GetComponent<room_logic>();
        return null;
    }

    //regenerates the rooms again if the previous generation failed
    private void regenerate_rooms()
    {
        //clear original generation
        rooms_list.ForEach(Destroy);
        rooms_list.Clear();
        room_grid = new int[grid_size_x, grid_size_y];
        room_queue.Clear();
        room_count = 0;
        generation_complete = false;
        end_rooms.Clear();
        enemy_rooms.Clear();

        //generate new rooms
        Vector2Int initial_room_index = new Vector2Int(grid_size_x / 2, grid_size_y / 2);
        StartRoomGenFromRoom(initial_room_index);
    }


    //STAGE TWO of generation
    private void generation_stage_two()
    {
        //allocate boss room, shop, item rooms
        List<GameObject> temp_end_rooms = end_rooms;

        //find furthest room to pick boss room
        int furthest_distance = 0;
        room_logic furthest_room = null;
        foreach(GameObject room in temp_end_rooms) //loop through all the end rooms
        {
            //calculate the distance from base room to the end room
            room_logic room_script = room.GetComponent<room_logic>();
            int x = room_script.room_index.x;
            int y = room_script.room_index.y;
            int distance = Mathf.Abs((grid_size_x / 2) - x) + Mathf.Abs((grid_size_y / 2) - y);

            //compare to the previous furthest
            if (distance > furthest_distance)
            {
                //new furthest room
                furthest_distance = distance;
                furthest_room = room_script;
                /*Debug.Log($"new furthest distance is {furthest_distance} at the index {room_script.room_index}");*/
            }
        }
        //set furthest room to boss room
        furthest_room.room_type = "boss";
        temp_end_rooms.Remove(furthest_room.gameObject);
        bossRoom = furthest_room.gameObject;

        //pick item rooms
        for(int i = 0; i < item_rooms ; i++)
        {
            GameObject item_room = temp_end_rooms[Random.Range(0,temp_end_rooms.Count)];
            temp_end_rooms.Remove(item_room);
            item_room.GetComponent<room_logic>().room_type = "item";
            item_rooms_list.Add(item_room);
        }

        //pick shop room
        shop_room = temp_end_rooms[Random.Range(0, temp_end_rooms.Count)];
        temp_end_rooms.Remove(shop_room);
        shop_room.GetComponent<room_logic>().room_type = "shop";

        //generation stage three
        generation_stage_three();
    }

    //count end rooms and alloctate basic room types
    private void determine_end_rooms()
    {
        //loop through rooms
        foreach (GameObject room in rooms_list)
        {
            room_logic room_script = room.GetComponent<room_logic>();
            if(room_script.room_index == new Vector2Int(grid_size_x / 2, grid_size_y / 2))
            {
                //starting room
                room_script.room_type = "start";
            }
            else if(count_adjancent_rooms(room_script.room_index) == 1)
            {
                //end room 
                end_rooms.Add(room);
                room_script.room_type = "end";
            }else
            {
                //enemy room
                enemy_rooms.Add(room);
                room_script.room_type = "enemy";
            }
        }
    }

    //STAGE THREE of generation
    private void generation_stage_three()
    {
        //spawn enemies and room configuations
        foreach(GameObject room in enemy_rooms) //loop through each enemy room
        {
            //room configuurations
            configController.getRoomConfig(room.transform);
            //spawn enemies
            room_logic room_script = room.GetComponent<room_logic>();
            Vector3 room_centre = GetPositionFromGridIndex(room_script.room_index);
            enemyGenController.floor_one_room_gen(room_script,room_centre); //run method in enemyGen script
        }

        //spawn item pedestals and items
        foreach (GameObject room in item_rooms_list) //loop through each enemy room
        {
            room_logic room_script = room.GetComponent<room_logic>(); //get reference to room
            Vector3 room_centre = GetPositionFromGridIndex(room_script.room_index);

            Instantiate(item_pedestal, room_centre, Quaternion.identity); //spawn pedestal

            int item = itemController.getItemFromItemPool(); //pick item

            GameObject instantiated_item = Instantiate(itemController.item_prefab, new Vector3(room_centre.x + itemController.item_library[item].item_x_offset, room_centre.y +itemController.item_library[item].item_y_offset,room_centre.z), Quaternion.identity); //spawn items

            instantiated_item.GetComponent<SpriteRenderer>().sprite = itemController.item_library[item].item_sprite; //set item sprite
            instantiated_item.GetComponent<itemLogic>().item_id = item; //set id correctly
        }

        //spawn room rewards
        List<room_logic> reward_rooms = new(); //setup keys

        foreach(GameObject room in enemy_rooms) //initialize list with enemy rooms
        {
            reward_rooms.Add(room.GetComponent<room_logic>());
        }

        List<string> item_drops = new(); //setup items to spawn
        for(int i = 0; i < 2; i++)
        {
            item_drops.Add("heart"); //base requriments for rewards is 2 hearts 3 coins
            item_drops.Add("coin");
        }
        item_drops.Add("coin");
        int reward_number = Random.Range(0, (int)Mathf.Round(stats.luck)) + 2; //additionally, 2-4 more rewards will spawn
        for (int i = 0; i < reward_number; i++)
        {
            switch (Random.Range(0, 5)) //pick a random room reward
            {
                case 0: //2 in 5 chance of heart
                case 1:
                    item_drops.Add("heart");
                    break;
                case 2: //2 in 5 chance of coins
                case 3:
                    item_drops.Add("coin");
                    break;
                case 4: //1 in 5 chance of spirit heart
                    item_drops.Add("spirit");
                    break;
            }
        }

        foreach(string pickup in item_drops) //spawn rewards
        {
            room_logic random_room = reward_rooms[Random.Range(0, reward_rooms.Count)]; //pick the index of a random room
            random_room.rewards.Add(pickup); //add reward to reward list of room
        }

        //spawn shop
        int shop_items = shopController.shop_number; //get number of shop items
        room_logic roomscript = shop_room.GetComponent<room_logic>(); //get reference to room
        Vector3 roomcentre = GetPositionFromGridIndex(roomscript.room_index);
        for (int i = 0; i < shop_items; i++) //spawn shop items
        {
            GameObject obj = Instantiate(shop_item, new Vector3(roomcentre.x + shop_slot_pos[i].x, roomcentre.y + shop_slot_pos[i].y, 0), Quaternion.identity); //instantiate item prefab
            obj.transform.GetChild(1).gameObject.GetComponent<shopItemLogic>().item_type = shopController.shop_items[i];
        }
        Instantiate(shopkeep, new Vector2(roomcentre.x - 0.072f, roomcentre.y + 2.16f), Quaternion.identity); //instantiate shopkeep

        //build minimap
        minimapController.instantiate_minimap();

        //boss room 
        room_logic boss_script = bossRoom.GetComponent<room_logic>();
        enemyGenController.spawn_boss_room(boss_script, GetPositionFromGridIndex(boss_script.room_index));

        //generation complete
        full_gen_complete = true;
    }

    public void addNewReward()
    {
        string rewardToAdd = "na";
        switch (Random.Range(0, 5)) //pick a random room reward
        {
            case 0: //2 in 5 chance of heart
            case 1:
                rewardToAdd = "heart";
                break;
            case 2: //2 in 5 chance of coins
            case 3:
                rewardToAdd = "coin";
                break;
            case 4: //1 in 5 chance of spirit heart
                rewardToAdd = "spirit";
                break;
        }

        List<room_logic> reward_rooms = new(); //setup room list
        foreach (GameObject room in enemy_rooms) //initialize list with enemy rooms
        {
            reward_rooms.Add(room.GetComponent<room_logic>());
        }
        room_logic random_room = reward_rooms[Random.Range(0, reward_rooms.Count)]; //pick the index of a random room
        random_room.rewards.Add(rewardToAdd); //add reward to reward list of room
    }
}
