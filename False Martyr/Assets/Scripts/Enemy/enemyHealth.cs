using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    //serialized variables
    public float enemy_health;
    [HideInInspector] public float max_health;
    [SerializeField] private bool is_boss;
    [SerializeField] private GameObject death_particles;
    [SerializeField] private GameObject death_particles2;
    public float spawn_check_radius;
    [SerializeField] private bool spawn_check_shown;
    [SerializeField] private GameObject bonusDeathPrefab;
    private bool is_dying = false;

    //references
    private ReferenceController reference;
    private camera_controller cam;
    private room_controller room;
    private bossHealthBar healthBar;

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
        healthBar = reference.bossHealthBar;

        //get components
        anim = GetComponent<Animator>();

        //set values
        max_health = enemy_health;
    }

    void Update()
    {
        if (enemy_health <= 0 && !is_dying) //check for death
            death();
        if (is_boss)
            healthBar.health = enemy_health;
            healthBar.maxHealth = max_health;

    }

    void death() //destroy object 
    {
        is_dying = true;
        if (TryGetComponent<minorDeity>(out minorDeity bossScript)) //checks if is minor deity
            StartCoroutine(minorDeityDeath(bossScript)); //special boss death
        
        else //normal death
        {
            Instantiate(death_particles, transform.position, Quaternion.identity);
            Instantiate(death_particles2, transform.position, Quaternion.identity);
            Instantiate(death_particles2, transform.position, Quaternion.identity);

            room.get_room_script_at_index(cam.room_index).enemy_count--; //decrement the enemy count of the current room

            Destroy(gameObject);
        }
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

    private IEnumerator minorDeityDeath(minorDeity bossScript)
    {
        bossScript.death.Invoke(); //destroy excess bullets

        GameObject deathAnim =  Instantiate(bonusDeathPrefab, transform.GetChild(0).GetChild(0).position, Quaternion.identity); //create death animation

        for(int i = 0; i < 3; i++) //destroy children
            Destroy(transform.GetChild(i).gameObject);

        bossScript.enabled = false; //disable all components
        bossScript.StopAllCoroutines();
        GetComponent<Rigidbody2D>().simulated = false;
        anim.enabled = false;

        Instantiate(death_particles, transform.position, Quaternion.identity); //explosion particles
        yield return new WaitForSeconds(0.9f);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.8333f);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.66666f);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.33333f);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.33333f);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.33333f);    

        Instantiate(death_particles, transform.position, Quaternion.identity); //death particles
        Instantiate(death_particles, transform.position, Quaternion.identity);
        Instantiate(death_particles2, transform.position, Quaternion.identity);
        Instantiate(death_particles2, transform.position, Quaternion.identity);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        Instantiate(death_particles, transform.position, Quaternion.identity);
        Instantiate(death_particles2, transform.position, Quaternion.identity);
        Instantiate(death_particles2, transform.position, Quaternion.identity);

        room.get_room_script_at_index(cam.room_index).enemy_count--; //decrement the enemy count of the current room
        Destroy(deathAnim); //destroy death anim
        Destroy(gameObject); //destroy this gameObject
    }
}
