using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class shopItemLogic : MonoBehaviour
{
    public int item_type;
    private int price;

    private ReferenceController reference;
    private shopController shopController;
    private player_coins playerCoins;
    private itemController itemController;
    private player_health playerHealth;

    [SerializeField] private GameObject particles;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        shopController = reference.ShopController;
        playerCoins = reference.Player.GetComponent<player_coins>();
        itemController = reference.ItemController;
        playerHealth = reference.Player.GetComponent<player_health>();

        price = shopController.shop_library[item_type].price;
        transform.parent.GetChild(0).GetComponent<TextMeshPro>().text = price.ToString();
        transform.localScale = new Vector2(shopController.shop_library[item_type].size, shopController.shop_library[item_type].size);
        transform.localPosition = new Vector2(transform.localPosition.x, shopController.shop_library[item_type].y_offset);
        GetComponent<SpriteRenderer>().sprite = shopController.shop_library[item_type].sprite;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //hit player
            if(playerCoins.coins >= price)
            {
                if((!(item_type == 0)) || (item_type == 0 && !(playerHealth.health == playerHealth.max_health))) //hearts cannot be picked up if on full health
                {
                    //buy item
                    playerCoins.gain_coins(price * -1); //charge money

                    if (shopController.shop_library[item_type].is_item) //check if it is an item
                    {
                        itemController.pickup_item(shopController.shop_library[item_type].item_index); //pickup item
                    }
                    else
                    {
                        pickup_logic(item_type); //use pickup
                    }

                    Instantiate(particles, transform.position, Quaternion.identity);
                    Destroy(transform.parent.gameObject); //destroy the game object
                }
            }
        }
    }

    void pickup_logic(int item_index)
    {
        //logic for non-pickup shop items
        if(item_type == 0)
        {
            //heart
            playerHealth.heal(1);
        }
        else if(item_type == 1)
        {
            //spirit heart
            playerHealth.get_spirit_hearts(1);
        }
    }
}
