using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderAttack : MonoBehaviour
{
    public float spin_speed;
    public float bullet_speed;
    [SerializeField] private GameObject damage_particle;
    private float start_time;
    private Vector3 start_pos;


    void Start()
    {
        start_time = Time.time;
        start_pos = transform.localPosition;
    }

    void Update()
    {
        if (Time.time < start_time + 0.2)
        {
            transform.localPosition = Vector3.Lerp(start_pos, new Vector3(start_pos.x, start_pos.y - 1.6f, start_pos.z), (Time.time - start_time) *5);
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, spin_speed * Time.deltaTime));
            transform.position += bullet_speed * Time.deltaTime * transform.right;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !(collision.gameObject.tag == "NoEnemyField"))
        {  //hit walls
            Instantiate(damage_particle, transform.position, Quaternion.identity); //instantiate particles
            transform.parent.GetComponent<autoAimEnemyProjectile>().destroy_bullet();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            player_health playerHealth = collision.gameObject.GetComponent<player_health>();
            if (!playerHealth.is_in_iframes)
                playerHealth.StartCoroutine(playerHealth.take_damage());
            Instantiate(damage_particle, transform.position, Quaternion.identity);
            transform.parent.GetComponent<autoAimEnemyProjectile>().destroy_bullet();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
            transform.parent.GetComponent<autoAimEnemyProjectile>().destroy_bullet(); //left camera
    }
}
