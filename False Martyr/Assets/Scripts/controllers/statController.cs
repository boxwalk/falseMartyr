using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statController : MonoBehaviour
{
    [Header("main stats")]
    public int maxhealth;
    public float damage;
    public float fireRate;
    [HideInInspector] public float trueFireRate; //true fire rate
    [HideInInspector] public int health;
    [HideInInspector] public int spiritHealth = 0;
    public float bulletSpeed;
    public float bulletSize;
    public float range;
    public float luck;
    public float arcana;
    public float greed;
    public float martyrism;
    public float fervour;
    [HideInInspector] public float i_frame_time;
    [HideInInspector] public float martyrism_damage_boost;
    [HideInInspector] public float temp_damage;
    [HideInInspector] public float final_damage;

    [Header("testing framework")]
    public bool testingForm;

    //references
    [HideInInspector] public room_controller room;

    private void Awake()
    {
        if(!testingForm)
            DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //set up values
        health = maxhealth;
        trueFireRate = 1 / (fireRate / 10);
    }

    void Update()
    {
        //check no stats are below their lower caps
        if (damage < 1)
            damage = 1;
        if (fireRate < 1)
            fireRate = 1;
        if (bulletSpeed < 1)
            bulletSpeed = 1;
        if (bulletSize < 1)
            bulletSize = 1;
        if (range < 1)
            range = 1;
        if (greed < 0)
            greed = 0;
        if (arcana < 0)
            arcana = 0;
        if (martyrism < 0)
            martyrism = 0;
        if (fervour < 0)
            fervour = 0;
        if (luck < 0)
            luck = 0;

        //calculate fire rate
        trueFireRate = 1 / (fireRate / 10);

        //calculate I frames
        i_frame_time = (martyrism * 0.25f) + 0.75f;

        //calculate martyrism damage boost
        martyrism_damage_boost = (martyrism - 3) * 0.25f;

        //calculate final damage
        final_damage = temp_damage + damage;
    }

    public void luckUp()
    {
        luck += 1;
        room.addNewReward();
    }

    public void luckDown()
    {
        luck -= 1;
    }
}
