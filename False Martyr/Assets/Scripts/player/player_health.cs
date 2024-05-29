using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_health : MonoBehaviour
{
     //serialized values
    [SerializeField] private float i_frames_duration;
    [SerializeField] private float shake_duration;
    [SerializeField] private float shake_magnitude;
    [SerializeField] private float wait_time_After_death;

    //private variables
    public bool is_in_iframes = false;
    public bool is_dead = false;

    //references
    private ReferenceController reference;
    private camera_controller cam;
    private gameplayController gameplay;
    private statController stats;
    [SerializeField] private GameObject damage_particles;
    [SerializeField] private GameObject death_particles;
    [SerializeField] private GameObject death2_particles;
    [SerializeField] private GameObject spirit_particles;

    //components
    private Animator anim;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        cam = reference.CameraController;
        gameplay = reference.GameplayController;
        stats = reference.StatController;
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
        if (stats.health + stats.spiritHealth <= 0) //check for death
            StartCoroutine(death());
        else{
            Instantiate(damage_particles, transform.position, Quaternion.identity); //damage particles
            yield return new WaitForSeconds(i_frames_duration);
            is_in_iframes = false; //end i frames
        }yield return new WaitForSeconds(0.1f);
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
        SceneManager.LoadScene(1); //load end screen
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Damage") && !is_in_iframes)
            StartCoroutine(take_damage());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Exit"))
            gameplay.nextFloor();
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
}
