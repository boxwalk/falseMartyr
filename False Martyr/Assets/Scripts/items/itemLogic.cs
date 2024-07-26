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
    [SerializeField] private float standard_scale;

    //references
    private ReferenceController reference;
    private itemController item;

    //particles
    [SerializeField] private GameObject pickup_particles;
    [SerializeField] private GameObject soulPickupParticles;
    [SerializeField] private GameObject aura_particles;
    [SerializeField] private GameObject heartSheen;
    [SerializeField] private GameObject lvUp_particles;


    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        item = reference.ItemController;
        //initialize variables
        root_y = transform.position.y;
        //unique star1t logic
        unique_start_logic();
        //scale
        StartCoroutine(Scale());
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
        if (item_id == 3) //heart of gold logic
            Instantiate(heartSheen, transform.position, Quaternion.identity, transform); //sheen
        if (item_id == 7) //LEVEL UP! logic
            Instantiate(lvUp_particles, transform.position, Quaternion.identity, transform); //aura particles
    }

    void unique_update_logic() //special logic for each item on update
    {

    }

    void hover() //sin wave to make the object bob up and down
    {
        float y_offset = Mathf.Sin(Time.time * speed) * magnitude;
        transform.position = new Vector2(transform.position.x, root_y + y_offset);
    }

    private IEnumerator Scale()
    {
        float scale = item.item_library[item_id].item_scale;
        float startTime = Time.time;
        Vector3 final_scale = new Vector3(scale,scale);
        while (Time.time < startTime + 1)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero,final_scale,Time.time - startTime);
            yield return null;
        }
        transform.localScale = new Vector3(scale, scale);
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.size = new Vector2((standard_scale / scale) * col.size.x, (standard_scale / scale) * col.size.y);
    }
}
