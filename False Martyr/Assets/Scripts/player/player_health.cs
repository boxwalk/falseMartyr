using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_health : MonoBehaviour
{
     //serialized values
    [SerializeField] private float shake_duration;
    [SerializeField] private float shake_magnitude;
    [SerializeField] private float wait_time_After_death;

    //private variables
    public bool is_in_iframes = false;
    public bool is_dead = false;
    private List<Transform> currentWards = new();

    //references
    private ReferenceController reference;
    private camera_controller cam;
    private gameController gameController;
    private statController stats;
    private Animator Ui_animator;
    [SerializeField] private GameObject damage_particles;
    [SerializeField] private GameObject death_particles;
    [SerializeField] private GameObject death2_particles;
    [SerializeField] private GameObject spirit_particles;
    [SerializeField] private GameObject ward_particles;

    //components
    private Animator anim;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        stats = reference.StatController;
        gameController = reference.GameController;
        Ui_animator = reference.UIController.gameObject.GetComponent<Animator>();
        //get components
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        check_limit_break();
    }

    void check_limit_break() //makes sure health is within thresholds
    {
        if (stats.maxhealth < stats.health)
            stats.health = stats.maxhealth;
        if (stats.health < 0)
            stats.health = 0;
        if (stats.spiritHealth < 0)
            stats.spiritHealth = 0;
    }

    public IEnumerator take_damage()
    {
        if (stats.passiveItemEffects.Contains("ward") && currentWards.Count != 0)
        {
            StartCoroutine(ward_damage_logic());
        }
        else
        {
            anim.SetTrigger("damage"); //start animation
            StartCoroutine(cam.camera_shake(shake_duration, shake_magnitude)); //camera shake

            if (stats.spiritHealth > 0) //take damage
            {
                Instantiate(spirit_particles, transform.position, Quaternion.identity);
                stats.spiritHealth--; //decrement spirit health
            }
            else
                stats.health--; //decrement health

            is_in_iframes = true; //start i frames
            if (stats.martyrism > 3)
            {
                Ui_animator.SetBool("martyrismSymbol", true); //martyrism symbol
                stats.temp_damage += stats.martyrism_damage_boost; //martyrism damage bonus
            }
            if (stats.health + stats.spiritHealth <= 0) //check for death
                StartCoroutine(death());
            else
            {
                Instantiate(damage_particles, transform.position, Quaternion.identity); //damage particles
                yield return new WaitForSeconds(stats.i_frame_time);
                is_in_iframes = false; //end i frames
                Ui_animator.SetBool("martyrismSymbol", false); //martyrism symbol
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator death()
    {
        //death
        is_dead = true;
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(0).gameObject.SetActive(false) ; //stop showing player
        for(int i = 0; i < 5; i++)
        {
            Instantiate(death2_particles, transform.position, Quaternion.identity); //death particles
            Instantiate(death_particles, transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(wait_time_After_death);
        SceneManager.LoadScene(2); //load end screen
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Damage") && !is_in_iframes)
            StartCoroutine(take_damage());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Exit"))
            gameController.nextFloor();
        else if (collision.gameObject.CompareTag("defensiveWard"))
            currentWards.Add(collision.gameObject.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("defensiveWard"))
            currentWards.Remove(collision.gameObject.transform);
    }

    public void max_health_up() //increment max health
    {
        stats.maxhealth++;
        stats.health++;
    }

    public void heal(int heal_amount) //heals the player by a certain amount
    {
        stats.health += heal_amount;
    }

    public void get_spirit_hearts(int amount) //gain spirit hearts
    {
        stats.spiritHealth += amount;
        Instantiate(spirit_particles, transform.position, Quaternion.identity);
    }

    private IEnumerator ward_damage_logic()
    {
        //delete ward
        float lowest_distance = float.MaxValue;
        Transform lowerst_distance_transform = null;
        foreach (Transform ward_transform in currentWards) //loop through current wards
        {
            float distance = Vector2.Distance(transform.position, ward_transform.position); //find distance
            if (distance < lowest_distance) //check if closest
            {
                lowerst_distance_transform = ward_transform;
                lowest_distance = distance;
            }
        }
        Destroy(lowerst_distance_transform.gameObject); //destroy gameObject

        //i frame logic
        is_in_iframes = true; //start i frames
        if (stats.martyrism > 3)
        {
            Ui_animator.SetBool("martyrismSymbol", true); //martyrism symbol
            stats.temp_damage += stats.martyrism_damage_boost; //martyrism damage bonus
        }
        Instantiate(ward_particles, transform.position, Quaternion.identity); //damage particles
        Instantiate(ward_particles, transform.position, Quaternion.identity); //damage particles
        Instantiate(death_particles, transform.position, Quaternion.identity); //damage particles
        yield return new WaitForSeconds(stats.i_frame_time);
        is_in_iframes = false; //end i frames
        Ui_animator.SetBool("martyrismSymbol", false); //martyrism symbol      
    }
}
