using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scytheProjectile : MonoBehaviour
{
    //serialized values
    public float bullet_speed;
    [SerializeField] private GameObject damage_particle;

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

        //start running logic
        StartCoroutine(mainBehaviour());
    }

    private IEnumerator mainBehaviour()
    {
        yield return new WaitForSeconds(3); // wiat for spawn animation

        transform.right = player.transform.position - transform.position; //"looks" at player
        rb.velocity = (transform.right).normalized * bullet_speed; //move towards player
        transform.rotation = Quaternion.identity; //reset rotation
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
            destroy_bullet(); //left camera
    }

    public void destroy_bullet()
    {
        Destroy(gameObject);
    }
}
