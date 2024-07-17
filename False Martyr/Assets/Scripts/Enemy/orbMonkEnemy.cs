using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbMonkEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    //movement values
    [SerializeField] private float speed;
    private bool ready_for_new_direction = true;
    private float move_rotation;
    private float directionResetTimer;
    [SerializeField] private float move_time_min;
    [SerializeField] private float move_time_max;
    [SerializeField] private LayerMask wall_mask;

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
        {
            enemy_behaviour();
        }
    }

    void enemy_behaviour()
    {
        //animation
        anim.SetBool("is_walking", true);

        //movement
        if (ready_for_new_direction) //new direction is needed
        {
            move_rotation = Random.Range(0f, 360f); //direction is randomly picked
            directionResetTimer = Time.time + Random.Range(move_time_min, move_time_max);
            ready_for_new_direction = false;
        }

        transform.rotation = Quaternion.Euler(0, 0, move_rotation); //point in direction
        rb.velocity = speed * transform.right; //move in direction
        transform.rotation = Quaternion.identity; //look back in original direction

        //check for wall hit
        Collider2D[] collision_list = Physics2D.OverlapCircleAll(transform.position, enemyHealth.spawn_check_radius, wall_mask);
        bool collision_with_wall = false;
        foreach (Collider2D coll in collision_list)
        {
            if (coll.gameObject.CompareTag("Wall"))
            {
                collision_with_wall = true;
                break;
            }
        }
        if (collision_with_wall) //hit wall, new direction
            ready_for_new_direction = true;

        if (!ready_for_new_direction && directionResetTimer < Time.time) //reset move direction
            ready_for_new_direction = true;
    }
}
