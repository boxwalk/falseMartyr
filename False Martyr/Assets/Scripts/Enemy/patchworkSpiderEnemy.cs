using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patchworkSpiderEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float speed;
    private float move_rotation;
    private bool is_attacking = false;
    private bool move_ready = true;
    [SerializeField] private float attack_recharge_min;
    [SerializeField] private float attack_recharge_max;
    private int attack_val;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject bullet_wall_prefab;

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
    }

    void enemy_behaviour()
    {
        //enemy behaviour
        if (is_attacking)
        {
            //attack
            rb.velocity = Vector2.zero;
            anim.SetBool("is_walking", false);
        }
        else
        {
            //movement
            if (move_ready)
            {
                move_rotation = Random.Range(0f, 360f); //direction is randomly picked
                move_ready = false;
                StartCoroutine(ready_attack());
            }
            anim.SetBool("is_walking", true);
            transform.rotation = Quaternion.Euler(0, 0, move_rotation); //point in direction
            rb.velocity = speed * transform.right; //move in direction
            transform.rotation = Quaternion.identity; //look back in original direction
        }
    }

    IEnumerator ready_attack()
    {
        yield return new WaitForSeconds(Random.Range(attack_recharge_min,attack_recharge_max));
        is_attacking = true;
        move_ready = true;
        StartCoroutine(attack());
    }

    IEnumerator attack()
    {
        attack_val = 1;
        //attack_val = Random.Range(0, 2); pick attack - uncomment this line to ewnable the second unused atatck
        if(attack_val == 0)
        {
            for(int i = 0; i <5; i++)
            {
                Instantiate(bullet_prefab, transform.position, Quaternion.identity); //spawn bullet
                yield return new WaitForSeconds(2f);
            }
            yield return new WaitForSeconds(2);
        }
        else
        {
            Instantiate(bullet_wall_prefab, transform.position, Quaternion.identity); //spawn bullet
            yield return new WaitForSeconds(1);
        }
        is_attacking = false;
    }
}
