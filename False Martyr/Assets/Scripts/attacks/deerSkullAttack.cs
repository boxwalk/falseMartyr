using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deerSkullAttack : MonoBehaviour
{
    //serialized values
    public float bullet_speed;
    public GameObject damage_particle;

    //components
    private Rigidbody2D rb;
    private GameObject player;

    //public values 
    public float dir;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>().Player;

        //set rotation
        //transform.Rotate(new Vector3(0f, 0f, dir));
        transform.right = player.transform.position - transform.position; //point towards player

        //destroy
        StartCoroutine(destroy());
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
        if (collision.gameObject.CompareTag("Player"))
        {
            player_health playerHealth = collision.gameObject.GetComponent<player_health>();
            if (!playerHealth.is_in_iframes)
                playerHealth.StartCoroutine(playerHealth.take_damage());
            Instantiate(damage_particle, playerHealth.transform.position, Quaternion.identity);
        }
    }

    private IEnumerator destroy()
    {
        yield return new WaitForSeconds(3);
        destroy_bullet();
    }

}
