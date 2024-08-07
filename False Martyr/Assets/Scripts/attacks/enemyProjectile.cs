using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{
    //serialized values
    public float bullet_speed;
    public GameObject damage_particle;

    //components
    private Rigidbody2D rb;

    //public values 
    public float dir;
    [HideInInspector] public bool spin = false;
    [HideInInspector] public float spin_speed;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();

        //set rotation
        transform.Rotate(new Vector3(0f, 0f, dir));
    }

    void Update()
    {
        if (spin)
            transform.Rotate(new Vector3(0, 0, spin_speed * Time.deltaTime));
        rb.velocity = transform.right * bullet_speed;
    }

    public void destroy_bullet()
    {
        Destroy(gameObject);
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

    public IEnumerator up_speed(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        bullet_speed *= 3;
    }
}
