using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    //movement variables
    [SerializeField] private float speed;
    private bool isAttacking = false;
    private bool ready_for_new_direction = true;
    private float move_rotation;
    private float directionResetTimer;
    [SerializeField] private float move_time_min;
    [SerializeField] private float move_time_max;
    [SerializeField] private LayerMask wall_mask;
    [SerializeField] private GameObject sword_sprite;
    [SerializeField] private List<float> swordSpriteXPositions;
    private int sword_positions;
    [SerializeField] private Transform centre_point;
    [SerializeField] private Transform test_point;
    [SerializeField] private float sword_radius;
    [SerializeField] private float attack_time_min;
    [SerializeField] private float attack_time_max;
    private bool first_behaviour = true;
    private float attackTimer;
    private Vector3 start_pos;
    private Vector3 end_pos;
    private Vector3 start_right;
    private Vector3 end_right;
    private Vector3 pre_right;
    [SerializeField] private GameObject slash;
    [SerializeField] private float sword_turn_offset;
    private bool aggresive_move = false; 
    private Vector3 move_right;

    //references
    private GameObject player;

    void Start()
    {
        start_logic(); //method from derived class
        //get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //get references
        player = reference.Player;
        //set up objects
        sword_positions = Random.Range(0, 2);
        sword_sprite.transform.localPosition = new Vector3(swordSpriteXPositions[sword_positions], sword_sprite.transform.localPosition.y, sword_sprite.transform.localPosition.z);
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
            anim.SetBool("animateSword", true);
        }
    }

    void enemy_behaviour()
    {
        if (first_behaviour)
        {
            //logic on first run
            first_behaviour = false;
            //set up values
            attackTimer = Time.time + Random.Range(attack_time_min, attack_time_max);
        }

        if (!isAttacking)
        {
            anim.SetBool("animateSword", true);
            anim.SetBool("is_walking", true); //animation

            //movement
            if (ready_for_new_direction) //new direction is needed
            {
                int aggresive_check = Random.Range(1, 3); //2 in 3 chance for aggressive move
                if (aggresive_check == 0)
                {
                    aggresive_move = false;
                    move_rotation = Random.Range(0f, 360f); //direction is randomly picked
                }else
                {
                    aggresive_move = true;
                    transform.right =  player.transform.position - transform.position;
                    move_right = transform.right;
                    transform.right = Vector2.right;
                }
                directionResetTimer = Time.time + Random.Range(move_time_min, move_time_max);
                ready_for_new_direction = false;
            }
            if (aggresive_move)
            {
                transform.right = move_right;
            }else
            {
                transform.rotation = Quaternion.Euler(0, 0, move_rotation); //point in direction
            }
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

            //attacking
            if (Time.time > attackTimer)
            {
                isAttacking = true;
                anim.SetBool("animateSword", false);
                //set up fade variables
                start_pos = sword_sprite.transform.position;
                start_right = sword_sprite.transform.up;
                sword_sprite.transform.position = centre_point.position;
                sword_sprite.transform.right = player.transform.position - sword_sprite.transform.position; //point towards player
                pre_right = sword_sprite.transform.right;
                sword_sprite.transform.position = centre_point.position + (sword_radius * sword_sprite.transform.right);
                end_pos = sword_sprite.transform.position;
                sword_sprite.transform.up = player.transform.position - sword_sprite.transform.position;
                end_right = sword_sprite.transform.up;
                sword_sprite.transform.position = start_pos;
                //slash posiitoning
                slash.transform.right = player.transform.position - sword_sprite.transform.position; //point towards player
                slash.transform.Rotate(new Vector3(0, 0, sword_turn_offset));
                //physics
                rb.velocity = Vector2.zero;
                //coroutine
                StartCoroutine(attack());
            }
        }
        else
        {
            anim.SetBool("is_walking", false); //animation
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator attack()
    {
        float attackStartTime = Time.time;
        while(attackStartTime + 1 > Time.time)
        {
            sword_sprite.transform.position = Vector3.Lerp(start_pos, end_pos, Time.time - attackStartTime);
            sword_sprite.transform.up = Vector3.Lerp(start_right, end_right, Time.time - attackStartTime);
            yield return null;
        }
        sword_sprite.transform.position = end_pos;
        sword_sprite.transform.up = end_right;
        float swing_magnitude = 50;
        float swing_progress = 0;
        float swing_time = 0.3f;
        while (attackStartTime + 1.3f > Time.time)
        {
            swing_progress += swing_magnitude * Time.deltaTime * (1/swing_time);
            sword_sprite.transform.position = centre_point.position;
            sword_sprite.transform.right = pre_right;
            sword_sprite.transform.Rotate(new Vector3(0, 0, swing_progress));
            sword_sprite.transform.position = centre_point.position + (sword_radius * sword_sprite.transform.right);
            sword_sprite.transform.up = sword_sprite.transform.right;
            yield return null;
        }
        swing_magnitude = 100;
        swing_progress = 0;
        swing_time = 0.2f;
        while (attackStartTime + 1.5f > Time.time)
        {
            swing_progress += swing_magnitude * Time.deltaTime * (1 / swing_time);
            sword_sprite.transform.position = centre_point.position;
            sword_sprite.transform.right = pre_right;
            sword_sprite.transform.Rotate(new Vector3(0, 0, 50 -swing_progress));
            sword_sprite.transform.position = centre_point.position + (sword_radius * sword_sprite.transform.right);
            sword_sprite.transform.up = sword_sprite.transform.right;
            yield return null;
        }

        end_pos = sword_sprite.transform.position;
        end_right = sword_sprite.transform.up;
        while (attackStartTime + 2.5f > Time.time)
        {
            sword_sprite.transform.position = Vector3.Lerp(end_pos, start_pos, Time.time - (attackStartTime +1.5f));
            sword_sprite.transform.up = Vector3.Lerp(end_right, start_right, Time.time - (attackStartTime + 1.5f));
            yield return null;
        }
        sword_sprite.transform.position = start_pos;
        sword_sprite.transform.up = start_right;

        isAttacking = false;
        anim.SetBool("animateSword", true);
        attackTimer = Time.time + Random.Range(attack_time_min, attack_time_max);
    }
}
