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
    [SerializeField] private List<GameObject> orbs;
    private List<Vector3> orb_positions = new();
    private List<Animator> orb_anims = new();
    [SerializeField] private float sin_wave_magnitude;
    [SerializeField] private float sin_wave_speed;
    [SerializeField] private float middle_orb_sin_wave_offset;
    [SerializeField] private GameObject bullet_prefab;
    private bool first_frame = true;
    [SerializeField] private float setAttackTimeplus0to4;
    [SerializeField] private Color bulletColour;
    [SerializeField] private GameObject bulletParticles;

    void Start()
    {
        start_logic(); //method from derived class
        //get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //setup values
        for(int i = 0; i < 3; i++)
        {
            orb_positions.Add(orbs[i].transform.localPosition);
            orb_anims.Add(orbs[i].GetComponent<Animator>());
        }

    }

    void Update()
    {
        update_logic(); //method from inherited class
        if (enemy_activated)
        {
            enemy_behaviour(); //run enemy behaviour if activated
        }
        //orbs
        for (int i = 0; i < 3; i++)
        {
            if(i == 2)
                orbs[i].transform.localPosition = new Vector2(orb_positions[i].x, orb_positions[i].y + (Mathf.Sin((Time.time + middle_orb_sin_wave_offset) * sin_wave_speed) * sin_wave_magnitude));
            else
                orbs[i].transform.localPosition = new Vector2(orb_positions[i].x, orb_positions[i].y + (Mathf.Sin(Time.time * sin_wave_speed) * sin_wave_magnitude));
        }
    }

    void enemy_behaviour()
    {
        if (first_frame)
        {
            first_frame = false;
            StartCoroutine(orb_attack());
        }

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

    private IEnumerator orb_attack()
    {
        yield return new WaitForSeconds(Random.Range(0, 5));
        yield return new WaitForSeconds(setAttackTimeplus0to4);
        //fire center orb
        spawn_attack(2);
        yield return new WaitForSeconds(2);
        //fire left orb
        spawn_attack(0);
        yield return new WaitForSeconds(2);
        //fire right orb
        spawn_attack(1);
        StartCoroutine(orb_attack());

    }

    private void spawn_attack(int num)
    {
        GameObject bullet = Instantiate(bullet_prefab, orbs[num].transform.position, Quaternion.identity);
        orb_anims[num].SetTrigger("attack");
        SpriteRenderer rend = bullet.GetComponent<SpriteRenderer>();
        rend.sortingOrder = 4;
        rend.color = bulletColour;
        bullet.GetComponent<autoAimEnemyProjectile>().damage_particle = bulletParticles;
    }
}
