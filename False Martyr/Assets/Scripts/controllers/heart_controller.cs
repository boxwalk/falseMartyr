using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heart_controller : MonoBehaviour
{
    //references
    private ReferenceController reference;
    private player_health playerHealth;

    //sprites
    [SerializeField] private Sprite empty_heart;
    [SerializeField] private Sprite full_heart;
    [SerializeField] private Sprite overload_heart;
    [SerializeField] private Sprite spirit_heart;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        playerHealth = reference.Player.GetComponent<player_health>();
    }

    void Update()
    {
        int health = playerHealth.health;
        int max_health = playerHealth.max_health;
        int spirit_hearts = playerHealth.spirit_health;
        for(int i = 1; i < 35; i++)
        {
            Image heart = transform.GetChild(i-1).gameObject.GetComponent<Image>();
            if(max_health + spirit_hearts < i)
                heart.enabled = false; //no heart
            else if (max_health < i){
                heart.enabled = true; //spirit heart
                heart.sprite = spirit_heart;}
            else if (health < i){
                heart.enabled = true; //empty heart
                heart.sprite = empty_heart;}
            else{
                heart.enabled = true; //full_heart
                heart.sprite = full_heart;}
        }

        if (max_health + spirit_hearts > 34) //check for overload heart
            transform.GetChild(33).gameObject.GetComponent<Image>().sprite = overload_heart;
    }
}
