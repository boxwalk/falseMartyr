using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupLogic : MonoBehaviour
{
    [SerializeField] private string pickup_type;
    [SerializeField] private GameObject pickup_particles;

    //references
    private ReferenceController reference;
    private GameObject player;
    private player_health player_health;
    private player_coins player_coins;

    void Start() 
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        player = reference.Player;
        player_health = player.GetComponent<player_health>();
        player_coins = player.GetComponent<player_coins>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && ((pickup_type == "heart" && player_health.health < player_health.max_health) || pickup_type == "spirit" || pickup_type == "coin")) //collided with player
        {
            Instantiate(pickup_particles, transform.position, Quaternion.identity); //spawn particles
            if(pickup_type == "heart")
            {
                player_health.heal(1); //heart picked up
            }
            else if(pickup_type == "spirit")
            {
                player_health.get_spirit_hearts(1); //spirit heart picked up
            }
            else if(pickup_type == "coin")
            {
                player_coins.gain_coins(1); //coin picked up
            }
            Destroy(gameObject);
        }
    }
}
