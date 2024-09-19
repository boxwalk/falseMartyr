using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class electricField : MonoBehaviour
{
    //values
    private float damage;
    [SerializeField] private GameObject explosion_particle;
    [SerializeField] private GameObject electric_particle;
    [SerializeField] private float max_flucuate_size;
    [SerializeField] private float min_flucuate_size;
    private float flucuate_size;
    private float extra_size_percentage;

    //references
    private ReferenceController reference;
    private statController stats;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = reference.StatController;

        //setup values
        damage = stats.arcana *2;
        extra_size_percentage = (stats.arcana - 5) * 0.075f;
        if (extra_size_percentage < 0)
            extra_size_percentage = 0;
        if (extra_size_percentage > 2.25f)
            extra_size_percentage = 2.25f;
        flucuate_size = Random.Range(min_flucuate_size, max_flucuate_size); //set size
        flucuate_size += extra_size_percentage;
        transform.localScale = new Vector2(transform.localScale.x*flucuate_size,transform.localScale.y*flucuate_size);
        StartCoroutine(mainLogic());

        GameObject particle_ = Instantiate(electric_particle, transform.position, Quaternion.identity); //particle
    }


    IEnumerator mainLogic()
    {
        yield return new WaitForSeconds(0.25f);
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, 2.2f*flucuate_size);
        foreach (Collider2D col in results)
        {
            if (col.gameObject.layer == 7) //enemy
            {
                col.gameObject.GetComponent<enemyHealth>().enemy_health -= damage; //deal damage
                col.gameObject.GetComponent<enemyHealth>().damage_flash(); // damage anim
                Instantiate(explosion_particle, transform.position, Quaternion.identity); //particle
            }
        }
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
}
