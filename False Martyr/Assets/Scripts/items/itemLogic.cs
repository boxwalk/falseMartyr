using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemLogic : MonoBehaviour
{
    //values
    public int item_id;
    private float root_y;
    private float magnitude = 0.15f;
    private float speed = 2.5f;

    //references
    private ReferenceController reference;
    private itemController item;

    //particles
    [SerializeField] private GameObject pickup_particles;
    [SerializeField] private GameObject soulPickupParticles;
    [SerializeField] private GameObject aura_particles;


    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        item = reference.ItemController;
        //initialize variables
        root_y = transform.position.y;
        //unique star1t logic
        unique_start_logic();
    }

    void Update()
    {
        unique_update_logic();
        hover();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            item.pickup_item(item_id); //pickup item
            Instantiate(pickup_particles, transform.position, Quaternion.identity); //pickup particles
            unique_pickup_logic(); //run unique pickup logic for each item
            Destroy(gameObject); //destroy item
        }
    }

    void unique_pickup_logic() //special logic for each item when picked up
    {
        if (item_id == 0) //grafted soul logic
            Instantiate(soulPickupParticles, transform.position, Quaternion.identity); //special particles
    }

    void unique_start_logic() //special logic for each item when instantiated
    {
        if (item_id == 0) //grafted soul logic
            Instantiate(aura_particles, transform.position, Quaternion.identity,transform); //aura particles
    }

    void unique_update_logic() //special logic for each item on update
    {

    }

    void hover() //sin wave to make the object bob up and down
    {
        float y_offset = Mathf.Sin(Time.time * speed) * magnitude;
        transform.position = new Vector2(transform.position.x, root_y + y_offset);
    }
}
