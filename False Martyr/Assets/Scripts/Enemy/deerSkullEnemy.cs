using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deerSkullEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    //movement
    [SerializeField] private float speed;
    private bool ready_for_new_direction = true;
    private float move_rotation;
    private float directionResetTimer;
    [SerializeField] private float move_time_min;
    [SerializeField] private float move_time_max;
    private float shootTimer;
    [SerializeField] private float shoot_time_min;
    [SerializeField] private float shoot_time_max;
    private bool is_shooting = false;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shoot_point;
    [SerializeField] private LayerMask wall_mask;
    private bool first_behaviour = true;

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
        enemy_behaviour();
        if (enemy_activated)
        {
            enemy_behaviour(); //run enemy behaviour if activated
        }
        else
        {
            //anim.SetBool("is_Walking", false);
        }
    }

    void enemy_behaviour()
    {
        //enemy behaviour
        if (first_behaviour)
        {
            //logic on first run
            first_behaviour = false;
            //set up values
            shootTimer = Time.time + Random.Range(shoot_time_min, shoot_time_max);
        }


        anim.SetBool("is_Walking", true); //animation

        if (!is_shooting)
        {
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

            //shooting
            if (Time.time > shootTimer)
            {
                shootTimer = Time.time + 6.5f; //reset shoot timer
                is_shooting = true;
                anim.SetTrigger("shoot");
                StartCoroutine(shoot()); //ready to shoot bullet
            }
        }
        else
        {
            //in the middle of shoot animation
            rb.velocity = Vector2.zero;
            if (Time.time > shootTimer)
            {
                shootTimer = Time.time + Random.Range(shoot_time_min, shoot_time_max); //shoot animation finished
                is_shooting = false;
            }
        }
    }

    private IEnumerator shoot()
    {
        yield return new WaitForSeconds(2);
        Instantiate(bullet, shoot_point.position, Quaternion.identity); //shoot bullet
        yield return new WaitForSeconds(4);
    }
}
