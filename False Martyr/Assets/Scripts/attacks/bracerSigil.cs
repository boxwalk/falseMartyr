using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bracerSigil : MonoBehaviour
{
    //values
    private float damage;
    [SerializeField] private GameObject explosion_particle;

    //references
    private ReferenceController reference;
    private statController stats;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = reference.StatController;

        //setup values
        damage = stats.damage / 2;

        /* old scale logic
        float scale = transform.localScale.x * (stats.bulletSize / 10);
        transform.localScale = new Vector2(scale, scale);
        */

        //start logic
        StartCoroutine(sigilLogic());
    }

    private IEnumerator sigilLogic()
    {
        for(int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(2);
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, 0.7f);
            //Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, 0.7f * (stats.bulletSize / 10)); //old logic
            foreach (Collider2D col in results)
            {
                if (col.gameObject.layer == 7) //enemy
                {
                    col.gameObject.GetComponent<enemyHealth>().enemy_health -= damage; //deal damage
                    col.gameObject.GetComponent<enemyHealth>().damage_flash(); // damage anim
                    Instantiate(explosion_particle, transform.position, Quaternion.identity); //particle
                }
            }
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
