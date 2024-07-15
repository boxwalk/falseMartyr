using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderWallAttack : MonoBehaviour
{
    private Vector3 start_pos;
    private float start_time;
    private bool first_frame = true;
    [SerializeField] private GameObject damage_particle;
    private SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.enabled = false;
    }

    void Update()
    {
        if (first_frame)
        {
            start_pos = transform.localPosition;
            transform.position = transform.parent.position;
            start_time = Time.time;
            first_frame = false;
            rend.enabled = true;
        }
        if (Time.time < start_time + 1)
        {
            transform.localPosition = Vector3.Lerp(Vector3.zero, start_pos, (Time.time - start_time));
        }
    }

    private void destroy_bullet()
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
}
