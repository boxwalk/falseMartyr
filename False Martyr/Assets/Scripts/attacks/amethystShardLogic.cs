using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class amethystShardLogic : MonoBehaviour
{
    //components
    private Rigidbody2D rb;

    //references
    private ReferenceController reference;
    private statController stats;

    //values
    [HideInInspector] public float dir;
    [SerializeField] private float bullet_speed;
    private float damage;
    [SerializeField] private GameObject explosion_particle;


    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();

        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = reference.StatController;

        damage = 1.5f + (stats.arcana * 0.5f);

        transform.Rotate(new Vector3(0f, 0f, dir));
    }

    void Update()
    {
        //set velocity of bullet
        rb.velocity = transform.right * bullet_speed;
    }

    void destroy_bullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !(collision.gameObject.tag == "NoEnemyField"))
        {  //hit walls
            Instantiate(explosion_particle, transform.position, Quaternion.identity); //instantiate particles
            destroy_bullet();
        }
        else if (collision.gameObject.layer == 7)
        {
            collision.gameObject.GetComponent<enemyHealth>().enemy_health -= damage; //hit enemies
            Instantiate(explosion_particle, transform.position, Quaternion.identity); //instantiate particles
            collision.gameObject.GetComponent<enemyHealth>().damage_flash(); // damage anim
            destroy_bullet();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
        {
            destroy_bullet(); //left camera
        }
    }
}
