using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batSummon : enemyAbstract
{
    //components
    private Rigidbody2D rb;

    //movement values
    [SerializeField] private float speed;
    private Transform player;

    void Start()
    {
        start_logic();
        rb = GetComponent<Rigidbody2D>();
        player = reference.Player.transform;
    }

    void Update()
    {
        update_logic();
        //enemy movement
        transform.right = player.position - transform.position; //"looks" at player
        rb.velocity = (transform.right).normalized * speed; //move towards player
        transform.rotation = Quaternion.identity; //reset rotation
    }
}
