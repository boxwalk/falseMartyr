using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sleepingEyeEnemy : enemyAbstract
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
    private bool is_attacking = false;
    [SerializeField] private List<CheckCollisionWithTag> attack_hitboxes;
    private float bullet_dir;
    [SerializeField] private GameObject bullet_prefab;
    private bool first_frame = true;
    private float start_time;

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
            start_time = Time.time;
        }
        if (!is_attacking)
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

            //check for attack
            bullet_dir = 1;
            if (attack_hitboxes[0].isTouching)
            {
                //left attack
                bullet_dir = 180;
            }
            else if (attack_hitboxes[1].isTouching)
            {
                //right attack
                bullet_dir = 0;
            }
            else if (attack_hitboxes[2].isTouching)
            {
                //up attack
                bullet_dir = 90;
            }
            else if (attack_hitboxes[3].isTouching)
            {
                //down attack
                bullet_dir = 270;
            }
            if((bullet_dir != 1) && (Time.time > start_time +3))
            {
                //attack
                anim.SetTrigger("attack");
                is_attacking = true;
                StartCoroutine(attack());
            }
        }
        else {
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator attack()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject bullet = Instantiate(bullet_prefab, transform.position, Quaternion.identity);
        bullet.GetComponent<enemyProjectile>().dir = bullet_dir;
        yield return new WaitForSeconds(1.21f);
        is_attacking = false;
    }
}
