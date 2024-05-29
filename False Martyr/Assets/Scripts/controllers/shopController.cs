using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class shopController : MonoBehaviour
{
    public List<shop_item> shop_library;

    public int shop_number;

    public List<int> shop_items = new List<int>();

    private ReferenceController reference;
    private statController stats;

    void Start()
    {
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = reference.StatController;

        shop_number = (int) (Random.Range(0, 5) + stats.luck); //get number of shop items

        //3 to 8 is limit, so check for limit breaks
        if (shop_number < 3)
            shop_number = 3;
        if (shop_number > 7)
            shop_number = 7;

        if (shop_number == 6) //fix lopsided shops
            shop_number = 5;
        else if (shop_number == 4)
            shop_number = 5;


        for (int i = 0; i < shop_number; i++)
        {
            shop_items.Add(Random.Range(0, 3)); //pick shop items
        }
    }

    [Serializable]
    public class shop_item
    {
        public Sprite sprite;
        public float y_offset;
        public float size;
        public int price;
        public bool is_item;
        public int item_index;    
    }
}
