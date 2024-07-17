using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ankhCultistEnemy : enemyAbstract
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
    [SerializeField] private float attack_time_min;
    [SerializeField] private float attack_time_max;
    private bool first_frame = true;
    private bool is_attacking = false;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private Transform shoot_point;
    private Transform player;
    [SerializeField] private Color bullet_color;

    void Start()
    {
        start_logic(); //method from derived class
        //get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = reference.Player.transform;
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
        if (!is_attacking)
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
            
            if (!ready_for_new_direction)
            {
                if (90 < move_rotation && move_rotation < 270) //face in correct direction
                {
                    transform.localScale = new Vector3(0.9f, 0.9f, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-0.9f, 0.9f, 1);
                }
            }
        }
        else
        {
            //animation
            anim.SetBool("is_walking", false);
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator attack()
    {
        yield return new WaitForSeconds(Random.Range(attack_time_min, attack_time_max));
        is_attacking = true;
        yield return new WaitForSeconds(1);
        GameObject bullet = Instantiate(bullet_prefab, shoot_point.position, Quaternion.identity);
        ankhProjectile bulletScript = bullet.GetComponent<ankhProjectile>();
        bulletScript.dir = pick_dir();
        yield return new WaitForSeconds(1);
        is_attacking = false;
        StartCoroutine(attack());
    }

    float pick_dir() //pick a new direction
    {
        int selected_dir = Random.Range(0, 2);
        if (selected_dir == 0) //up or down
            selected_dir = (transform.position.y > player.transform.position.y) ? 3 : 1; //shoots in the direction of player 
        else //left or right
            selected_dir = (transform.position.x > player.transform.position.x) ? 2 : 0; //shoots in the direction of player 
        return selected_dir*90; //return direction
    }
}
