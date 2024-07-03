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
            enemy_behaviour();
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
                sword_sprite.transform.position = centre_point.position + (sword_radius * sword_sprite.transform.right);
                end_pos = sword_sprite.transform.position;
                sword_sprite.transform.up = player.transform.position - sword_sprite.transform.position;
                end_right = sword_sprite.transform.up;
                sword_sprite.transform.position = start_pos;
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
    }
}
