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
    [SerializeField] private List<GameObject> extra_prefabs;
    [SerializeField] private int lucky_item = 0;

    //references
    private ReferenceController reference;
    private GameObject player;
    private UIController ui;
    private statController stats;
    private shopController shop;
    private room_controller room;
    private camera_controller cam;

    //temp variables
    private List<int> temp_item_pool = new();
    private GameObject greed_bonus;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        player = reference.Player;
        ui = reference.UIController;
        stats = reference.StatController;
        shop = reference.ShopController;
        room = reference.RoomController;
        cam = reference.CameraController;
        greed_bonus = player.GetComponent<player_coins>().greedBonus;

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
        if(lucky_item != 0)
        {
            temp_item_pool.Add(lucky_item); //lucky item logic for testing
            lucky_item = 0;
        }
        else
        { //actual algorithm here
            for (int i = 0; i < item_library.Count; i++)
            {
                if (item_library[i].is_unlimited_supply || item_library[i].supply > 0)
                    temp_item_pool.Add(i);
            }
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

    public void pickup_item(int itemIndex, bool greedable) //item picked up
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
            case 4:
                reinforcedOrgans();
                break;
            case 5:
                condensedBlessing();
                break;
            case 6:
                luckyPebble();
                break;
            case 7:
                levelUp();
                break;
            case 8:
                amuletOfVitality();
                break;
            case 9:
                heartOfThorns();
                break;
            case 10:
                ritualDagger();
                break;
            case 11:
                hauntedTome();
                break;
            case 12:
                giantsBlood();
                break;
            case 13:
                arcaneSigil();
                break;
            case 14:
                falseIdol();
                break;
            case 15:
                sharpTooth();
                break;
            case 16:
                ravensFeather();
                break;
            case 17:
                sacredButcheringKnife();
                break;
            case 18:
                covetedPearl();
                break;
            case 19:
                holySeal();
                break;
            case 20:
                wigglyWorm();
                break;
            case 21:
                twoFacedMask();
                break;
            case 22:
                boneFracture();
                break;
            case 23:
                sigilCraftersBracers();
                break;
            case 24:
                deathbringersScythe();
                break;
            case 25:
                azureAmethyst();
                break;
            case 26:
                fluctuatingCore();
                break;

            default: //mathcing item not found
                Debug.Log("error: no item found");
                break;
        }

        //modification
        if (item_library[itemIndex].has_mod)
            reference.ModificationController.create_modification(itemIndex); //create mod

        //flavour text
        //ui.flavourText(item_library[itemIndex].item_name, item_library[itemIndex].flavour_texts[Random.Range(0, item_library[itemIndex].flavour_texts.Count)]); //get name and flavour text
        ui.flavourText(item_library[itemIndex].item_name, item_library[itemIndex].flavour_texts[0]); //for now, only descriptive flavour texts

        //greed bonus
        int greed_chance = Random.Range(1, 101);
        if (greed_chance < stats.greed_extra_item_chance && greedable)
        {
            //greed proc
            int pos = Random.Range(0, 2); //pick x posiition
            float x_pos;
            Vector3 room_centre = room.GetPositionFromGridIndex(cam.room_index);
            if (pos == 0)
                x_pos = 5;
            else
                x_pos = -5;

            //spawn item
            Instantiate(room.item_pedestal, new Vector2(room_centre.x + x_pos, room_centre.y), Quaternion.identity); //spawn pedestal
            int item = getItemFromItemPool(); //pick item
            GameObject instantiated_item = Instantiate(item_prefab, new Vector3(room_centre.x + item_library[item].item_x_offset + x_pos, room_centre.y + item_library[item].item_y_offset, room_centre.z), Quaternion.identity); //spawn items
            instantiated_item.GetComponent<SpriteRenderer>().sprite = item_library[item].item_sprite; //set item sprite
            itemLogic itemScript = instantiated_item.GetComponent<itemLogic>(); //get script
            itemScript.item_id = item; //set id 
            itemScript.greedable = false;


            Instantiate(greed_bonus, player.transform.position, Quaternion.identity); //particle
        }
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
        //greed down
        stats.greed -= 2;
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

    void reinforcedOrgans()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //bullet speed up
        stats.bulletSpeed += 5;
        //fire rate down
        stats.fireRate -= 2;
    }

    void condensedBlessing()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //fervour up
        stats.fervour += 3;
        //greed down
        stats.greed -= 2;
    }
    void luckyPebble()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //luck up
        stats.luckUp();
        //bullet speed down
        stats.bulletSpeed -= 2;
    }
    void levelUp()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //bullet size up
        stats.bulletSize += 2;
        //martyrism down
        stats.martyrism -= 1;
    }
    void amuletOfVitality()
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //arcana up
        stats.arcana += 3;
        //martyrism down
        stats.martyrism -= 1;
    }
    void heartOfThorns() //now renamed throny heart
    {
        //health up
        player.GetComponent<player_health>().max_health_up();
        //martyrism up
        stats.martyrism += 2;
        //fervour down
        stats.fervour -= 2;
    }
    void ritualDagger()
    {
        //damage up
        stats.damage += 4;
    }
    void hauntedTome()
    {
        //fire rate up
        stats.fireRate += 3;
    }
    void giantsBlood()
    {
        //bullet size up
        stats.bulletSize += 8;
        //bullet effect
        stats.passiveItemEffects.Add("giantsBlood");
    }
    void arcaneSigil()
    {
        //arcana up
        stats.arcana += 10;
    }
    void falseIdol()
    {
        //greed up
        stats.greed += 6;
        //extra mods
        Instantiate(extra_prefabs[0], player.transform.GetChild(0).GetChild(1));
        Instantiate(extra_prefabs[1], player.transform.GetChild(0).GetChild(2));
    }
    void sharpTooth()
    {
        //damage up
        stats.damage += 3;
        //luck up
        stats.luckUp();
        //bullet speed down
        stats.bulletSpeed -= 2;
    }
    void ravensFeather()
    {
        //arcana up
        stats.arcana += 8;
        //range up
        stats.range += 5;
        //luck down
        stats.luckDown();
    }
    void sacredButcheringKnife()
    {
        //martyrism up
        stats.martyrism += 5;
        //greed up
        stats.greed += 4;
        //range down
        stats.arcana -= 4;
    }
    void covetedPearl()
    {
        //fire rate up
        stats.fireRate += 2;
        //greed up
        stats.greed += 3;
        //damage down
        stats.damage -= 1;
    }
    void holySeal()
    {
        //fervour up
        stats.fervour += 5;
        //arcana up
        stats.arcana += 4;
        //greed down
        stats.greed -= 2;
    }
    void wigglyWorm()
    {
        //damage up
        stats.damage += 1;
        //fire rate up
        stats.fireRate += 3;
        //bullet speed down
        stats.bulletSpeed -= 4;
        //bullet effect
        stats.passiveItemEffects.Add("worm");
    }
    void twoFacedMask()
    {
        //range down
        stats.range -= 7;
        //bullet effect
        stats.passiveItemEffects.Add("twinMask");
    }
    void boneFracture()
    {
        //bullet size up
        stats.bulletSize += 5;
        //bullet effect
        stats.passiveItemEffects.Add("fracture");
    }
    void sigilCraftersBracers()
    {
        //bullet effect
        stats.passiveItemEffects.Add("bracers");
    }
    void deathbringersScythe()
    {
        //bullet size up
        stats.bulletSize += 5;
        //bullet speed up
        stats.bulletSpeed += 3;
        //fire rate down
        stats.fireRate -= 2;
        //damage up
        stats.damage += 5;
        //bullet effect
        stats.passiveItemEffects.Add("scythe");
    }
    void azureAmethyst()
    {
        //bullet effect
        stats.passiveItemEffects.Add("amethyst");
    }
    void fluctuatingCore()
    {
        //passive effect
        stats.passiveItemEffects.Add("electricCore");
        StartCoroutine(player.GetComponent<player_attacks>().electricCore_logic()); //start coroutine
    }

}
