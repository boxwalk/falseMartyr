using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack_logic : MonoBehaviour
{
    //public variables
    public int attack_dir = 1;

    //serialized values
    [SerializeField] private float bullet_speed;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosion_particle;
    [SerializeField] private GameObject damage_particle;

    //components
    private Rigidbody2D rb;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();
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
}
