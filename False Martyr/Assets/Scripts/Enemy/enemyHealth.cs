using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    //serialized variables
    public float enemy_health;
    [SerializeField] private GameObject death_particles;
    [SerializeField] private GameObject death_particles2;
    public float spawn_check_radius;
    [SerializeField] private bool spawn_check_shown;

    //references
    private ReferenceController reference;
    private camera_controller cam;
    private room_controller room;

    //componeents
    private Animator anim;

    //variables for abstract
    [HideInInspector] public Vector2Int room_index = new Vector2Int(1000,1000);

    void Start()
    {
        //get refrences
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        room = reference.RoomController;

        //get components
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemy_health <= 0) //check for death
            death();
    }

    void death() //destroy object 
    {
        Instantiate(death_particles, transform.position, Quaternion.identity);
        Instantiate(death_particles2, transform.position, Quaternion.identity);
        Instantiate(death_particles2, transform.position, Quaternion.identity);

        room.get_room_script_at_index(cam.room_index).enemy_count--; //decrement the enemy count of the current room

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        //draw spawn sphere
        if(spawn_check_shown)
            Gizmos.DrawWireSphere(transform.position, spawn_check_radius);
    }

    public void damage_flash()
    {
        anim.SetTrigger("damage");
    }
}
