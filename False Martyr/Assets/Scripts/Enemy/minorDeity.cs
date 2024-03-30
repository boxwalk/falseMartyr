using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minorDeity : enemyAbstract
{
    void Start()
    {
        start_logic(); //method from derived class
    }

    void Update()
    {
        update_logic(); //method from derived class
        if (enemy_activated)
        {
            enemy_behaviour(); //run enemy logic if activated
        }
    }

    void enemy_behaviour()
    {

    }
}
