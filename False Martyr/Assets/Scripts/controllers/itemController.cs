using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class itemController : MonoBehaviour
{
    [Serializable]
    public class item //custom class
    {
        public string item_name;
        public Sprite item_sprite;
        public string item_key;
        public bool is_unlimited_supply;
        public int supply;
        public float item_y_offset;
        public bool has_mod;
        public GameObject modification_prefab;
        public string mod_parent;
        public List<string> flavour_texts;
    }

    //variable decleration
    public List<item> item_library;
    public GameObject item_prefab;

    //references
    private ReferenceController reference;
    private GameObject player;
    private UIController ui;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        player = reference.Player;
        ui = reference.UIController;
    }

    public int getItemFromItemPool() //pick item from item pool
    {
        return 0;
    }

    public void pickup_item(int itemIndex) //item picked up
    {
        //run pickup item logic
        switch (itemIndex) //switcher with index
        {
            case 0: //check each possible index and run the correct logic, eg. grafted soul is index 0
                grafted_soul();
                break;

            case 1:
                break;

            default: //mathcing item not found
                Debug.Log("error: no item found");
                break;
        }

        //modification
        if (item_library[itemIndex].has_mod)
            reference.ModificationController.create_modification(itemIndex); //create mod

        //flavour text
        ui.flavourText(item_library[itemIndex].item_name, item_library[itemIndex].flavour_texts[Random.Range(0, item_library[itemIndex].flavour_texts.Count)]); //get name and flavour text
    }

    void grafted_soul() //logic on pickup of grated soul
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
    }
}
