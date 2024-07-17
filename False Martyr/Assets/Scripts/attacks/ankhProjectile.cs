using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ankhProjectile : MonoBehaviour
{
    //serialized values
    public float bullet_speed;
    [SerializeField] private GameObject damage_particle;
    [SerializeField] private CheckCollisionWithTag[] cols;
    private bool bonus_shot = false;
    [SerializeField] GameObject bullet;

    //components
    private Rigidbody2D rb;
    private SpriteRenderer rend;

    //public values 
    public float dir;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        //set rotation
        transform.Rotate(new Vector3(0f, 0f, dir));
        rb.velocity = transform.right * bullet_speed;
        transform.right = Vector2.right;

        if (!bonus_shot)
        {
            if (cols[0].isTouching && !(dir == 90 || dir == 270))
            {
                //down
                instantiate_bullet(90);
                instantiate_bullet(270);
                bonus_shot = true;
            }
            else if (cols[1].isTouching && !(dir == 90 || dir == 270))
            {
                //up
                instantiate_bullet(90);
                instantiate_bullet(270);
                bonus_shot = true;
            }
            else if (cols[2].isTouching && !(dir == 0 || dir == 180 ))
            {
                //left
                instantiate_bullet(0);
                instantiate_bullet(180);
                bonus_shot = true;
            }
            else if (cols[3].isTouching && !(dir == 0 || dir == 180))
            {
                //right
                instantiate_bullet(0);
                instantiate_bullet(180);
                bonus_shot = true;
            }
        }
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

    private void instantiate_bullet(float dir)
    {
        bullet = Instantiate(bullet, transform.position, Quaternion.identity);
        bullet.GetComponent<SpriteRenderer>().color = rend.color;
        enemyProjectile bullet_script = bullet.GetComponent<enemyProjectile>();
        bullet_script.dir = dir;
        bullet_script.damage_particle = damage_particle;
        bullet_script.bullet_speed = 8;
    }
}
