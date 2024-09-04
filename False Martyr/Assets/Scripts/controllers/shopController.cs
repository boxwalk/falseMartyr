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


    [Serializable]
    public class shop_item
    {
        public Sprite sprite;
        public float y_offset;
        public float x_offset;
        public float size;
        public int price;
        public int item_index;
    }
}
