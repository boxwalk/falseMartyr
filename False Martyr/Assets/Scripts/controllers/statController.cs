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
    public float potency;
    public float sin;
    public float fervour;

    //references
    [HideInInspector] public room_controller room;

    private void Awake()
    {
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
        //calculate fire rate
        trueFireRate = 1 / (fireRate / 10);
    }

    public void luckUp()
    {
        luck += 1;
        room.addNewReward();
    }
}
