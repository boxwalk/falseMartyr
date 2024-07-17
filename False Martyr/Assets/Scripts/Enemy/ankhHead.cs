using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ankhHead : enemyAbstract
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
    private bool first_frame = true;
    [SerializeField] private GameObject bullet_prefab;

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
        if (first_frame)
        {
            first_frame = false;
            StartCoroutine(attack());
        }
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

    IEnumerator attack()
    {
        yield return new WaitForSeconds(2.5f);
        //attack
        anim.SetTrigger("attack");
        for(int i = 0; i < 360; i+= 90) //shoot bullets in the 4 cardinal directions
        {
            GameObject bullet = Instantiate(bullet_prefab, transform.position, Quaternion.identity);
            bullet.GetComponent<enemyProjectile>().dir = i;
        }
        StartCoroutine(attack());
    }
}
