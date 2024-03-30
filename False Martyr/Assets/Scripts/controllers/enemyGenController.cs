using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class enemyGenController : MonoBehaviour
{
    [Serializable]
    public class enemy_data //custom class to hold enemy data
    {
        public string name;
        public GameObject prefab;
    }

    //enemies
    [SerializeField] private List<enemy_data> enemy_library;

    //layermasks
    [SerializeField] private LayerMask environemnt;

    //offsets
    private float upper_y_offset = 3.5f;
    private float lower_y_offset = -3.5f;
    private float left_x_offset = -7.3f;
    private float right_x_offset = 7.3f;

    public void floor_one_room_gen(room_logic room_script, Vector3 room_centre)
    {
        //generate enemies
        int enemy_number = Random.Range(3, 6); //get the number of enemies
        room_script.enemy_count = enemy_number; //set the number of enemies in the room script

        for(int i = 0; i < enemy_number; i++)
        {
            //instantiate enemies
            GameObject instantiated_enemy = Instantiate(enemy_library[0].prefab, new Vector2(room_centre.x + Random.Range(left_x_offset,right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset)), Quaternion.identity);

            //set values in instantiated enemy
            instantiated_enemy.GetComponent<enemyHealth>().room_index = room_script.room_index;

            while (Physics2D.OverlapCircle(instantiated_enemy.transform.position,instantiated_enemy.GetComponent<enemyHealth>().spawn_check_radius,environemnt)) //check the enemies are not in walls or by the doors
                instantiated_enemy.transform.position = new Vector2(room_centre.x + Random.Range(left_x_offset, right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset));
        }
    }

    public void spawn_boss_room(room_logic room_script, Vector3 room_centre)
    {
        room_script.enemy_count = 1; //set enemy count to 1
        
        //instantiate enemies
        GameObject instantiated_enemy = Instantiate(enemy_library[1].prefab, new Vector2(room_centre.x + Random.Range(left_x_offset, right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset)), Quaternion.identity);

        //set values in instantiated enemy
        instantiated_enemy.GetComponent<enemyHealth>().room_index = room_script.room_index;

        while (Physics2D.OverlapCircle(instantiated_enemy.transform.position, instantiated_enemy.GetComponent<enemyHealth>().spawn_check_radius, environemnt)) //check the enemies are not in walls or by the doors
            instantiated_enemy.transform.position = new Vector2(room_centre.x + Random.Range(left_x_offset, right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset));
    }
}

