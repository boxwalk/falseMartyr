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
    public GameObject player_attack;

    //serialized values
    private float fire_speed;

    //attack variables
    private bool attack_charged = true;

    //transforms
    [SerializeField] private Transform shoot_point;
    [SerializeField] private Transform top_shoot_point;
    [SerializeField] private Transform bottom_shoot_point;

    //references
    private player_health playerHealth;
    private ReferenceController reference;
    private gameController gameController;
    private statController statController;


    void Start()
    {
        //get references
        playerHealth = GetComponent<player_health>();
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        gameController = reference.GameController;
        statController = reference.StatController;
    }

    void Update()
    {
        if (!playerHealth.is_dead && gameController.fullGameStart)
        {
            get_stats();
            gather_input();
            fire_attacks();
        }
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
            int attack_dir = 0;
            if (is_right_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, shoot_point.position, Quaternion.identity); //Instantiate bullet
                attack_prefab.GetComponent<attack_logic>().attack_dir = 1;  //right attack
                attack_dir = 1;
            }
            else if (is_left_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, shoot_point.position, Quaternion.identity); //Instantiate bullet
                attack_prefab.GetComponent<attack_logic>().attack_dir = 2;  //left attack
                attack_dir = 2;
            }
            else if (is_up_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, top_shoot_point.position, Quaternion.identity); //Instantiate bullet
                attack_prefab.GetComponent<attack_logic>().attack_dir = 3;  //up attack
                attack_dir = 3;
            }
            else if (is_down_arrow_pressed)
            {
                GameObject attack_prefab = Instantiate(player_attack, bottom_shoot_point.position, Quaternion.identity);
                attack_prefab.GetComponent<attack_logic>().attack_dir = 4;  //down attack
                attack_dir = 4;
            }

            attack_charged = false; 
            StartCoroutine(attack_recharge());
            //special attack item logic
            if (statController.passiveItemEffects.Contains("twinMask"))
                StartCoroutine(twoFacedMaskLogic(attack_dir));
        }
    }

    IEnumerator attack_recharge()
    {
        yield return new WaitForSeconds(fire_speed); //wait for attack to recharge
        attack_charged = true;
    }

    void get_stats()
    {
        fire_speed = statController.trueFireRate; //get stat from stats controller
    }

    IEnumerator twoFacedMaskLogic(int dir)
    {
        if(!statController.passiveItemEffects.Contains("worm")) //wiggly worm synergy
            yield return new WaitForSeconds(fire_speed/10); //wait for a tenth of fire rate
        GameObject attack_prefab = Instantiate(player_attack, shoot_point.position, Quaternion.identity); //Instantiate bullet
        attack_logic bulletScript = attack_prefab.GetComponent<attack_logic>();
        bulletScript.attack_dir = dir;
        if (statController.passiveItemEffects.Contains("worm")) //wiggly worm synergy
            bulletScript.isTwinMaskSynegy = true;
    }
}
