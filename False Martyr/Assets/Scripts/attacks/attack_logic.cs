using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack_logic : MonoBehaviour
{
    //public variables
    public int attack_dir = 1;

    //serialized values
    private float bullet_speed;
    private float damage;
    [SerializeField] private GameObject explosion_particle;
    [SerializeField] private GameObject damage_particle;

    //components
    private Rigidbody2D rb;
    private Animator anim;

    //reference
    private ReferenceController reference;
    private statController stats;

    //main values
    private float baseSize;
    private float range;
    private float initializedTime;
    private bool rangeAnimStarted = false;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = reference.StatController;

        //set up characteristics
        damage = stats.final_damage;
        range = stats.range;
        bullet_speed = stats.bulletSpeed;
        baseSize = transform.localScale.x;
        float newScale = baseSize * (stats.bulletSize / 10);
        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
        initializedTime = Time.time;
    }

    void Update()
    {
        //set velocity of bullet
        if (attack_dir == 1)
            rb.velocity = new Vector2(bullet_speed,0); //attack right
        else if (attack_dir == 2)
            rb.velocity = new Vector2(-bullet_speed, 0); //attack left
        else if (attack_dir == 3)
            rb.velocity = new Vector2(0, bullet_speed); //attack up
        else if (attack_dir == 4)
            rb.velocity = new Vector2(0, -bullet_speed); //attack down

        rangeLogic();
    }

    void destroy_bullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !(collision.gameObject.tag == "NoEnemyField")){  //hit walls
            Instantiate(explosion_particle, transform.position, Quaternion.identity); //instantiate particles
            destroy_bullet();}
        else if (collision.gameObject.layer == 7){
            collision.gameObject.GetComponent<enemyHealth>().enemy_health -= damage; //hit enemies
            Instantiate(damage_particle, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<enemyHealth>().damage_flash(); // damage anim
            destroy_bullet(); }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
            destroy_bullet(); //left camera
    }
    
    void rangeLogic()
    {
        if(((Time.time - initializedTime + 0.5f)*bullet_speed) > range && !rangeAnimStarted)
        {
            //start playing range anim
            anim.SetTrigger("rangeOut");
            rangeAnimStarted = true;
        }
        else if (((Time.time - initializedTime) * bullet_speed) > range)
        {
            destroy_bullet();
        }
    }
}
