using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        start_logic(); //method from derived class
        //get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        update_logic(); //method from inherited class
        if (enemy_activated)
        {
            enemy_behaviour(); //run enemy behaviour if activated
        }
        else
            anim.SetBool("is_Walking", false);
    }

    void enemy_behaviour()
    {
        //enemy behaviour here
    }
}
