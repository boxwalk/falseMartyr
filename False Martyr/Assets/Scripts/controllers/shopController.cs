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

    void Start()
    {
        shop_number = Random.Range(3, 8); //get number of shop items
        for(int i = 0; i < shop_number; i++)
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
