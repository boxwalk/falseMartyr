using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_attacks : MonoBehaviour
{
    //input booleans
    private bool is_left_arrow_pressed;
    private bool is_right_arrow_pressed;
    private bool is_up_arrow_pressed;
    private bool is_down_arrow_pressed;

    //prefab references
    [SerializeField] private GameObject player_attack;

    //serialized values
    [SerializeField] private float fire_speed;

    //attack variables
    private bool attack_charged = true;

    //transforms
    [SerializeField] private Transform shoot_point;
    [SerializeField] private Transform top_shoot_point;
    [SerializeField] private Transform bottom_shoot_point;

    void Start()
    {
        
    }

    void Update()
    {
        gather_input();
        fire_attacks();
    }

    void gather_input()
    {
        //gather input
        is_left_arrow_pressed = Input.GetKey(KeyCode.LeftArrow);
        is_right_arrow_pressed = Input.GetKey(KeyCode.RightArrow);
        is_down_arrow_pressed = Input.GetKey(KeyCode.DownArrow);
        is_up_arrow_pressed = Input.GetKey(KeyCode.UpArrow);
    }

    void fire_attacks()
    {
        if (attack_charged && (is_left_arrow_pressed || is_right_arrow_pressed || is_down_arrow_pressed || is_up_arrow_pressed))
        {
            if (is_right_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, shoot_point.position, Quaternion.identity); //Instantiate bullet
                attack_prefab.GetComponent<attack_logic>().attack_dir = 1;  //right attack
            }
            else if (is_left_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, shoot_point.position, Quaternion.identity); //Instantiate bullet
                attack_prefab.GetComponent<attack_logic>().attack_dir = 2;  //left attack
            }
            else if (is_up_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, top_shoot_point.position, Quaternion.identity); //Instantiate bullet
                attack_prefab.GetComponent<attack_logic>().attack_dir = 3;  //up attack
            }
            else if (is_down_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, bottom_shoot_point.position, Quaternion.identity);
                attack_prefab.GetComponent<attack_logic>().attack_dir = 4;  //down attack
            }

            attack_charged = false; 
            StartCoroutine(attack_recharge());
        }
    }

    IEnumerator attack_recharge()
    {
        yield return new WaitForSeconds(fire_speed); //wait for attack to recharge
        attack_charged = true;
    }
}
