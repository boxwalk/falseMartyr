using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopItemLogic : MonoBehaviour
{
    public int item_type;

    private ReferenceController reference;
    private shopController shopController;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        shopController = reference.ShopController;

        transform.localScale = new Vector2(shopController.shop_library[item_type].size, shopController.shop_library[item_type].size);
        transform.localPosition = new Vector2(transform.localPosition.x, shopController.shop_library[item_type].y_offset);
        GetComponent<SpriteRenderer>().sprite = shopController.shop_library[item_type].sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
