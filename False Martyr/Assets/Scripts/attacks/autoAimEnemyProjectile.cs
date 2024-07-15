using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoAimEnemyProjectile : MonoBehaviour
{
    //serialized values
    public float bullet_speed;
    [SerializeField] private GameObject damage_particle;
    [SerializeField] private bool is_spider_attack = false;
    [SerializeField] private bool is_multiSpider_attack = false;

    //components
    private Rigidbody2D rb;

    //references
    private ReferenceController reference;
    private GameObject player;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        player = reference.Player;

        transform.right = player.transform.position - transform.position; //point towards player

        if (is_multiSpider_attack)
        {
            Destroy(transform.GetChild(Random.Range(0, 4)).gameObject);
        }
    }

    void Update()
    {
        rb.velocity = transform.right * bullet_speed;
    }

    public void destroy_bullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_spider_attack)
        {
            if (collision.gameObject.layer == 6 && !(collision.gameObject.tag == "NoEnemyField"))
            {  //hit walls
                Instantiate(damage_particle, transform.position, Quaternion.identity); //instantiate particles
                destroy_bullet();
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                player_health playerHealth = collision.gameObject.GetComponent<player_health>();
                if (!playerHealth.is_in_iframes)
                    playerHealth.StartCoroutine(playerHealth.take_damage());
                Instantiate(damage_particle, transform.position, Quaternion.identity);
                destroy_bullet();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
            destroy_bullet(); //left camera
    }
}
