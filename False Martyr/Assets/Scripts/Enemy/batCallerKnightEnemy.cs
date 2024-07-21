using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class batCallerKnightEnemy : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    //values
    private bool first_frame = true;
    [SerializeField] private float speed;
    [SerializeField] private float move_min;
    [SerializeField] private float move_max;
    [SerializeField] private float attack_min;
    [SerializeField] private float attack_max;
    [SerializeField] private float idle_min;
    [SerializeField] private float idle_max;
    private float move_rotation;
    [SerializeField] private LayerMask wall_mask;
    [SerializeField] private GameObject bat;
    [HideInInspector] public UnityEvent death;

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
            StartCoroutine(bat_summon());
            StartCoroutine(movement());
        }

    }


    private IEnumerator movement()
    {
        //walk
        anim.SetBool("is_walking", true);
        move_rotation = Random.Range(0f, 360f); //direction is randomly picked

        float move_time = Random.Range(move_min, move_max);
        float timeElapsed = 0;

        while(timeElapsed < move_time)
        {
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
                move_rotation = Random.Range(0f, 360f);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //idle
        anim.SetBool("is_walking", false);
        rb.velocity = Vector2.zero;
       yield return new WaitForSeconds(Random.Range(idle_min, idle_max));

        StartCoroutine(movement());
    }

    private IEnumerator bat_summon()
    {
        yield return new WaitForSeconds(Random.Range(attack_min, attack_max));
        //summon bat
        anim.SetTrigger("attack");
        GameObject _bat = Instantiate(bat, transform.position, Quaternion.identity);
        death.AddListener(_bat.GetComponent<enemyHealth>().drop_dead);
        yield return new WaitForSeconds(1/3);
        StartCoroutine(bat_summon());
    }
}
