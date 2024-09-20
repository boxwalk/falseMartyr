using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostlyEssence : MonoBehaviour
{
    //main values
    [SerializeField] private float speed;
    private bool can_damage = true;
    private float damage;
    [SerializeField] private GameObject damage_particle;
    private float base_x_scale;
    private GameObject player_body;

    //components
    private Rigidbody2D rb;
    private Animator anim;
    private ReferenceController reference;
    private GameObject player;
    private statController stats;

    //inputs
    private bool left_key_pressed;
    private bool right_key_pressed;
    private bool up_key_pressed;
    private bool down_key_pressed;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        player = reference.Player;
        stats = reference.StatController;

        //setup damage
        damage = stats.arcana * 1.5f;

        //animation values
        player_body = transform.GetChild(0).gameObject;
        base_x_scale = player_body.transform.localScale.x;
    }

    void Update()
    {
        gather_input();
        playerMovement();
    }

    //player movement 
    void playerMovement()
    {
        //horizontal movement
        float x_velocity = 0f;
        if (left_key_pressed)
        { x_velocity = -1f; }
        else if (right_key_pressed)
        { x_velocity = 1f; }

        //vertical movement
        float y_velocity = 0f;
        if (up_key_pressed)
        { y_velocity = 1f; }
        else if (down_key_pressed)
        { y_velocity = -1f; }

        //normalize vector
        Vector2 movement_dir = new Vector2(x_velocity, y_velocity).normalized;

        //animations
        if (movement_dir == Vector2.zero)
            anim.SetBool("is_walking", false);
        else
            anim.SetBool("is_walking", true);     

        //set to rigidbody velocity
        rb.velocity = (movement_dir * speed);

        //point in direction of movement
        if (rb.velocity.x > 0)
            player_body.transform.localScale = new Vector3(-base_x_scale, transform.localScale.y, transform.localScale.z);
        else if (rb.velocity.x < 0)
            player_body.transform.localScale = new Vector3(base_x_scale, transform.localScale.y, transform.localScale.z);
    }

    //gather input
    void gather_input()
    {
        //directional inputs
        left_key_pressed = Input.GetKey(KeyCode.LeftArrow);
        right_key_pressed = Input.GetKey(KeyCode.RightArrow);
        up_key_pressed = Input.GetKey(KeyCode.UpArrow);
        down_key_pressed = Input.GetKey(KeyCode.DownArrow);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
        {
            transform.position = player.transform.position;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && can_damage)
        {
            collision.gameObject.GetComponent<enemyHealth>().enemy_health -= damage; //hit enemies
            Instantiate(damage_particle, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<enemyHealth>().damage_flash(); // damage anim
            can_damage = false;
            StartCoroutine(damageRefresh());
        }
    }

    IEnumerator damageRefresh()
    {
        yield return new WaitForSeconds(1);
        can_damage = true;
    }
}

