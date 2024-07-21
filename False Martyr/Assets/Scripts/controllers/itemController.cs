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
        public float item_scale;
        public float item_x_offset;
        public float item_y_offset;
        public bool is_in_shop;
        public int shop_index;
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
    private statController stats;
    private shopController shop;

    //temp variables
    private List<int> temp_item_pool = new();

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        player = reference.Player;
        ui = reference.UIController;
        stats = reference.StatController;
        shop = reference.ShopController;

        //Shop item gen
        shop.shop_number = (int)(Random.Range(0, 5) + stats.luck); //get number of shop items
        //3 to 8 is limit, so check for limit breaks
        if (shop.shop_number < 3)
            shop.shop_number = 3;
        if (shop.shop_number > 7)
            shop.shop_number = 7;

        if (shop.shop_number == 6) //fix lopsided shops
            shop.shop_number = 5;
        else if (shop.shop_number == 4)
            shop.shop_number = 5;

        //get number of hearts, spirit hearts and items
        int heart_num = 0;
        int spirit_num = 0;
        int item_num = 0;
        if(shop.shop_number == 3)
        {
            heart_num = Random.Range(1, 3);
            spirit_num = 1;
            item_num = 2 - heart_num;
        }
        else if (shop.shop_number == 5)
        {
            heart_num = Random.Range(2,4);
            spirit_num = 4 - heart_num - Random.Range(0,2);
            item_num = 5 - spirit_num - heart_num;
        }
        else if (shop.shop_number == 7)
        {
            heart_num = Random.Range(2, 5);
            spirit_num = 5 - heart_num - Random.Range(0, 2);
            item_num = 7 - spirit_num - heart_num;
        }
        //add hearts
        for (int i = 0; i < heart_num; i++)
        {
            shop.shop_items.Add(0);
        }
        //add spirit hearts
        for (int i = 0; i < spirit_num; i++)
        {
            shop.shop_items.Add(1);
        }
        //add items
        refillItemPoolforShop();
        for(int i = 0; i < item_num; i++)
        {
            if (temp_item_pool.Count == 0)
                refillItemPoolforShop();
            int picked_item = temp_item_pool[Random.Range(0, temp_item_pool.Count)];
            if (!item_library[picked_item].is_unlimited_supply)
                item_library[picked_item].supply--;
            temp_item_pool.Remove(picked_item);
            shop.shop_items.Add(item_library[picked_item].shop_index);
        }

        //item pool ready
        refillItemPool();
    }

    private void refillItemPool()
    {
        temp_item_pool.Clear();
        for (int i = 0; i < item_library.Count; i++)
        {
            if(item_library[i].is_unlimited_supply || item_library[i].supply > 0)
                temp_item_pool.Add(i);
        }
    }

    private void refillItemPoolforShop()
    {
        temp_item_pool.Clear();
        for (int i = 0; i < item_library.Count; i++)
        {
            if ((item_library[i].is_unlimited_supply || item_library[i].supply > 0) && item_library[i].is_in_shop)
                temp_item_pool.Add(i);
        }
    }

    public int getItemFromItemPool() //pick item from item pool
    {
        if (temp_item_pool.Count == 0)
            refillItemPool();
        int picked_item = temp_item_pool[Random.Range(0, temp_item_pool.Count)];
        if (!item_library[picked_item].is_unlimited_supply)
            item_library[picked_item].supply--;
        temp_item_pool.Remove(picked_item);
        return picked_item;
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
                abnormal_growth();
                break;
            case 2:
                ham_sandwich();
                break;
            case 3:
                heartOfGold();
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

    void abnormal_growth() //logic on pickup of abnormal growth
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //luck down
        stats.luckDown();
        //damage up
        stats.damage += 1;
    }

    void ham_sandwich()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //bullet speed up
        stats.bulletSpeed += 3;
        //damage down
        stats.damage -= 1;
    }

    void heartOfGold()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //greed up
        stats.greed += 3;
        //bullet speed down
        stats.bulletSpeed -= 2;
    }
}
