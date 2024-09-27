using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class minimapRoom : MonoBehaviour
{
    //components
    private RectTransform rect;
    private Image image;
    private Image haze;

    //public values
    public Vector2Int room_index;
    public string room_type;
    [SerializeField] private GameObject teleportParticles;

    //references
    private minimapController minimap;
    private room_controller RoomController;
    private ReferenceController reference;
    private gameplayController gameplay;
    private GameObject player;
    private camera_controller cam;

    //sprites
    [SerializeField] private Sprite discovered;
    [SerializeField] private Sprite entered;
    [SerializeField] private Sprite active;

    //children
    [SerializeField] private Image boss;
    [SerializeField] private Image item;
    [SerializeField] private Image shop;

    //components
    private Animator ui_anim;
    private Animator minimap_anim;

    void Start()
    {
        //get components
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        haze = transform.parent.GetChild(0).gameObject.GetComponent<Image>();
        ui_anim = transform.parent.parent.parent.gameObject.GetComponent<Animator>();
        minimap_anim = transform.parent.parent.gameObject.GetComponent<Animator>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        minimap = reference.MinimapController;
        RoomController = reference.RoomController;
        gameplay = reference.GameplayController;
        player = reference.Player;
        cam = reference.CameraController;
    }

    void Update()
    {
        //rendering and sprite
        if (minimap.discovered_rooms.Contains(room_index)) //check if room discovered
        {
            image.enabled = true; //enable rendering
            if(minimap.room_index == room_index)
                image.sprite = active; //active room
            else if (minimap.entered_rooms.Contains(room_index))
                image.sprite = entered; //room entered
            else
                image.sprite = discovered; //room discovered
        }else
        {
            image.enabled = false; //room not discovered
        }

        //set alphas
        image.color = new Color(image.color.r, image.color.g, image.color.b, haze.color.a);
        boss.color = new Color(image.color.r, image.color.g, image.color.b, haze.color.a); 
        item.color = new Color(image.color.r, image.color.g, image.color.b, haze.color.a); 
        shop.color = new Color(image.color.r, image.color.g, image.color.b, haze.color.a); 

        Vector3 pos = new Vector3(minimap.room_x * (room_index.x - RoomController.grid_size_x / 2) *minimap.scale, minimap.room_y * (room_index.y - RoomController.grid_size_y / 2) * minimap.scale); //calculates transform from index
        rect.anchoredPosition = pos; //set transform

        //set scale
        rect.localScale = new Vector3(0.08f * minimap.scale, 0.08f * minimap.scale, 1);

        //set room type 
        if(image.enabled == true)
        {
            if (room_type == "boss")
                boss.enabled = true; //boss room
            else if (room_type == "item")
                item.enabled = true; //item room
            else if (room_type == "shop")
                shop.enabled = true; //shop room
        }else
        {
            boss.enabled = false; //room not shown
            item.enabled = false;
            shop.enabled = false;
        }

        //if minimap disabled, no prompt
        if(haze.color.a != 1)
            ui_anim.SetBool("telePromptIn", false);
    }

    public void button_press()
    {
        if (minimap.entered_rooms.Contains(room_index) && !(minimap.room_index == room_index) && gameplay.enemy_room_clear && !minimap_anim.GetBool("is_teleporting")) //room entered, not current room, not in battle
        {
            //teleport
            minimap_anim.SetTrigger("teleport");
            minimap_anim.SetBool("is_teleporting", true);
            StartCoroutine(teleport());
        }
    }

    IEnumerator teleport()
    {
        yield return new WaitForSeconds(0.6f);
        cam.room_index = room_index;
        float x_offset = 0;
        if (room_type == "item")
        {
            x_offset = Random.Range(0, 2);
            if (x_offset == 0)
                x_offset = 5;
            else
                x_offset = -5;
        }
        else if (room_type == "shop" || room_type == "boss")
        {
            x_offset = Random.Range(0, 2);
            if (x_offset == 0)
                x_offset = 7;
            else
                x_offset = -7;
        }
        player.transform.position = RoomController.GetPositionFromGridIndex(room_index);
        player.transform.position = new Vector2(player.transform.position.x + x_offset, player.transform.position.y);
        cam.gameObject.transform.position = RoomController.GetPositionFromGridIndex(room_index);
        yield return new WaitForSeconds(0.2f);
        Instantiate(teleportParticles, player.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        minimap_anim.SetBool("is_teleporting", false);
    }

    public void mouseOver()
    {
        if (minimap.entered_rooms.Contains(room_index) && !(minimap.room_index == room_index) && gameplay.enemy_room_clear) //room entered, not current room, not in battle
        {
            ui_anim.SetBool("telePromptIn", true);
        }
    }

    public void mouseExit()
    {
        ui_anim.SetBool("telePromptIn", false);
    }

}
