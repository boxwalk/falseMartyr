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

    //references
    private minimapController minimap;
    private room_controller RoomController;
    private ReferenceController reference;

    //sprites
    [SerializeField] private Sprite discovered;
    [SerializeField] private Sprite entered;
    [SerializeField] private Sprite active;

    //children
    [SerializeField] private Image boss;
    [SerializeField] private Image item;
    [SerializeField] private Image shop;

    void Start()
    {
        //get components
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        haze = transform.parent.GetChild(0).gameObject.GetComponent<Image>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        minimap = reference.MinimapController;
        RoomController = reference.RoomController;
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

    }
}
