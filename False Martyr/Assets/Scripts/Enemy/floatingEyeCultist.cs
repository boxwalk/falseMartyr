using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingEyeCultist : enemyAbstract
{
    //components
    private Rigidbody2D rb;
    private Animator anim;

    //movement values
    [SerializeField] private float speed;
    private Transform player;
    private Vector3 move_right;
    private bool first_frame = true;
    [SerializeField] private float attack_min;
    [SerializeField] private float attack_max;
    private bool is_attacking = false;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private float bullet_spread;
    [SerializeField] private float bullet_speed;

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
            StartCoroutine(Attack());
            transform.right = player.transform.position - transform.position;
            move_right = transform.right;
            transform.right = Vector2.right;
        }
        if (!is_attacking)
        {
            anim.SetBool("is_walking", true);
            rb.velocity = speed * move_right;
        }
        else
        {
            anim.SetBool("is_walking", false);
            rb.velocity = Vector2.zero;
        }

    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(Random.Range(attack_min, attack_max));
        //attack
        is_attacking = true;
        yield return new WaitForSeconds(2);
        //shoot
        shoot_with_offset(0);
        shoot_with_offset(bullet_spread);
        shoot_with_offset(-bullet_spread);
        yield return new WaitForSeconds(2);
        transform.right = player.transform.position - transform.position;
        move_right = transform.right;
        transform.right = Vector2.right;
        is_attacking = false;
        StartCoroutine(Attack());
    }

    void shoot_with_offset(float offset)
    {
        GameObject bullet = Instantiate(bullet_prefab, transform.position, Quaternion.identity);
        autoAimEnemyProjectile bulletScript = bullet.GetComponent<autoAimEnemyProjectile>();
        bulletScript.offset = offset;
        bulletScript.bullet_speed = bullet_speed;
    }
}

