using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skeletonEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;
    private CheckCollisionWithTag attack_check;

    //references
    private GameObject player;

    //main values
    [SerializeField] private float moveSpeed;
    private bool is_attacking = false;

    void Start()
    {
        start_logic(); //method from derived class

        //get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        attack_check = transform.GetChild(1).GetChild(2).GetComponent<CheckCollisionWithTag>();

        //get references
        player = reference.Player;
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
        if (!is_attacking)
        {
            //enemy movement
            transform.right = player.transform.position - transform.position; //"looks" at player
            rb.velocity = (transform.right).normalized * moveSpeed; //move towards player
            transform.rotation = Quaternion.identity; //reset rotation

            //point towards player
            if (player.transform.position.x < transform.position.x)
                anim.SetBool("facingLeft", false);
            else
                anim.SetBool("facingLeft", true);

            //check for attack
            if (attack_check.isTouching)
            {
                //attack
                is_attacking = true;
                rb.velocity = Vector2.zero;
                StartCoroutine(resolve_attack());
                anim.SetTrigger("attack");
            }
        }
        else
        {
            rb.velocity = Vector2.zero; //not moving
        }
    }

    private IEnumerator resolve_attack()
    {
        yield return new WaitForSeconds(1);
        is_attacking = false;
    }
}