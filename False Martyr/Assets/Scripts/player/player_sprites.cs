using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_sprites : MonoBehaviour
{
    //referneces and components
    private GameObject player_body;
    private Rigidbody2D rb;
    private Animator anim;

    private float base_x_scale;

    void Start()
    {
        //get references
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        player_body = transform.GetChild(0).gameObject;
        base_x_scale = player_body.transform.localScale.x;
    }

    void Update()
    {
        //point in direction of movement
        if (rb.velocity.x > 0)
            player_body.transform.localScale = new Vector3(-base_x_scale, transform.localScale.y, transform.localScale.z);
        else if (rb.velocity.x < 0)
            player_body.transform.localScale = new Vector3(base_x_scale, transform.localScale.y, transform.localScale.z);

        //play animations
        if (rb.velocity == new Vector2(0, 0))
            anim.SetBool("is_walking", false);
        else
            anim.SetBool("is_walking", true);
    }
}
