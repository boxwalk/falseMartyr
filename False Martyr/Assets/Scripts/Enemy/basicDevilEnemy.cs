using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicDevilEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    //movement variables
    [SerializeField] private float speed;
    private int move_dir;
    [SerializeField] private float min_walk_time;
    [SerializeField] private float max_walk_time;
    private bool is_walking = false;
    private float time_walking;
    private float walk_target;
    private float base_x_scale;
    private float pause_time;
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private float jump_distance;
    private bool is_in_range;
    private bool is_jumping;
    private bool prev_frame_range;

    //points
    [SerializeField] private Transform top_point;
    [SerializeField] private Transform bottom_point;
    [SerializeField] private Transform left_point;
    [SerializeField] private Transform right_point;

    //references
    private GameObject player;


    void Start()
    {
        start_logic(); //method from derived class
        //get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //initialize variables
        base_x_scale = transform.localScale.x;
        pause_time = Random.Range(0f, 1.5f);
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
        //move in cardinal direction
        if (!is_walking && !is_jumping){
            if (pause_time <= 0){
                rb.velocity = Vector2.zero; //set velocity to zero
                move_dir = pick_dir(); //pick direction
                is_walking = true;
                walk_target = Random.Range(min_walk_time, max_walk_time);
                time_walking = 0;}
            else
                rb.velocity = Vector2.zero;
        }else if(!is_jumping){

            //move in direction
            if (move_dir == 0){ //move up
                rb.velocity = new Vector2(0, speed);
                check_for_walls(top_point.position, 1);
            }
            else if (move_dir == 1){ //move down
                rb.velocity = new Vector2(0, -speed);
                check_for_walls(bottom_point.position, 0);
            }
            else if (move_dir == 2){ //move left
                rb.velocity = new Vector2(-speed, 0);
                transform.localScale = new Vector2(base_x_scale, transform.localScale.y);
                check_for_walls(left_point.position, 3);
            }
            else if (move_dir == 3){ //move right
                rb.velocity = new Vector2(speed, 0);
                transform.localScale = new Vector2(-base_x_scale, transform.localScale.y);
                check_for_walls(left_point.position, 2);
            }

             //increment time
             time_walking += Time.deltaTime;
             //check for end of walk
             if (time_walking > walk_target){
                is_walking = false;
                pause_time = Random.Range(0, 2);
                pause_time = (pause_time == 1) ? Random.Range(0f, 1.5f): 0f;} //50 percent chance of pause, otherwise no pause  
        }

        //check if close
        is_in_range = (Vector2.Distance(transform.position, player.transform.position) < jump_distance);

        //check for jump chance
        if(is_in_range && !prev_frame_range && !is_jumping)
        {
            bool jump_chance = (Random.Range(0, 2) == 0); //50 percent chance to jump
            if (jump_chance)
                StartCoroutine(jump()); //jump
        }

        //animator update
        anim.SetBool("is_walking", is_walking);

        //decrement wait time
        pause_time -= Time.deltaTime;

        //set previous frame variables
        prev_frame_range = is_in_range;
    }

    void check_for_walls(Vector3 pos, int flip_dir) //check for flip if there are walls
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 0.1f, ground_mask);
        foreach (Collider2D col in results)
        {
            if (col.gameObject.tag == "Wall") //wall detected
            {
                move_dir = flip_dir; //flip
                time_walking = 0;
                if(move_dir == 0)
                    rb.velocity = new Vector2(0, speed);
                else if (move_dir == 1)
                    rb.velocity = new Vector2(0, -speed);
                else if (move_dir == 2)
                    rb.velocity = new Vector2(-speed,0);
                else if (move_dir == 3)
                    rb.velocity = new Vector2(speed,0);

            }
        }
    }

    int  pick_dir() //pick a new direction
    {
        int selected_dir = Random.Range(0, 2);
        if(selected_dir == 0) //up or down
            selected_dir = (transform.position.y > player.transform.position.y) ? 1 : 0; //moves in the direction of player 
        else //left or right
            selected_dir = (transform.position.x > player.transform.position.x) ? 2 : 3; //moves in the direction of player 
        return selected_dir; //return direction
    }

    private IEnumerator jump() //jump logic
    {
        is_jumping = true; //begin jump
        rb.velocity = Vector2.zero;
        anim.SetTrigger("jump"); //start animation

        Vector2 target_pos = player.transform.position; //set target

        yield return new WaitForSeconds(1); //wait for windup

        float elapsed_time = 0; //initilaize jumping variables
        Vector3 start_pos = transform.position;

        while(elapsed_time < 0.84f) //0.84 is duration of jump anim
        {
            transform.position = Vector2.Lerp(start_pos, target_pos, (elapsed_time / 0.84f)); //interpolates between the start and end position
            elapsed_time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = target_pos; //reach end position
        yield return new WaitForSeconds(0.4f); //wait for end of anim
        time_walking = 0; //reset walking time

        is_jumping = false; //end jump
    }

    private void OnDrawGizmos() //draw points as gizmos
    {
        Gizmos.DrawWireSphere(top_point.position, 0.1f);
        Gizmos.DrawWireSphere(bottom_point.position, 0.1f);
        Gizmos.DrawWireSphere(left_point.position, 0.1f);
        Gizmos.DrawWireSphere(right_point.position, 0.1f);
    }
}
