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

    [Serializable]
    public class enemy_presets //custom class to hold enemy presets
    {
        public string name;
        public int weight;
        public int spawnCountMin;
        public int spawnCountMax;
        public List<int> basicEnemies;
        public List<int> mediumRarityEnemies;
        public List<int> RareEnemies;
        public List<int> CenterPieceEnemies;
        public List<int> SwarmEnemies;
        public int swarmMultiplierMin;
        public int swarmMultiplierMax;
    }

    //enemies
    [SerializeField] private List<enemy_data> enemy_library;
    [SerializeField] private List<enemy_presets> preset_library;
    [SerializeField] private List<enemy_presets> floor2preset_library;
    [SerializeField] private List<int> bossPool;

    //layermasks
    [SerializeField] private LayerMask environment;

    //offsets
    private float upper_y_offset = 3.5f;
    private float lower_y_offset = -3.5f;
    private float left_x_offset = -7.3f;
    private float right_x_offset = 7.3f;

    public void floor_one_room_gen(room_logic room_script, Vector3 room_centre, float floor)
    {
        //generate enemies
        List<int> preset_picking = new(); //initialize list used for preset selection

        int enemyPreset = 0;
        enemy_presets preset = null;

        //pick preset
        if (floor == 1)
        {
            for (int i = 0; i < preset_library.Count; i++) //fill preset selection list
            {
                for (int j = 0; j < preset_library[i].weight; j++)
                {
                    preset_picking.Add(i);
                }
            }
            enemyPreset = preset_picking[Random.Range(0, preset_picking.Count)]; //pick preset
            preset = preset_library[enemyPreset]; //get preset
        }
        else
        {
            for (int i = 0; i < floor2preset_library.Count; i++) //fill preset selection list
            {
                for (int j = 0; j < floor2preset_library[i].weight; j++)
                {
                    preset_picking.Add(i);
                }
            }
            enemyPreset = preset_picking[Random.Range(0, preset_picking.Count)]; //pick preset
            preset = floor2preset_library[enemyPreset]; //get preset
        }

        int enemy_number = Random.Range(preset.spawnCountMin, preset.spawnCountMax + 1); //get the number of enemies
        int enemiesToSpawnCount = enemy_number;
        List<GameObject> enemiesToSpawn = new();


        if (!(preset.CenterPieceEnemies.Count == 0)) //check for centerpiece enemies
        {
            for(int i = 0; i < preset.CenterPieceEnemies.Count; i++) //add centerpiece enemies
            {
                enemiesToSpawn.Add(enemy_library[ preset.CenterPieceEnemies[i]].prefab); //add to list of enemies to spawn
                enemiesToSpawnCount--;
            }
        }

        List<string> enemyTypes = new();
        int enemyTypesCount;
        string enemyTypeToSpawn;
        int swarmToSpawn;
        for (int i = 0; i < enemiesToSpawnCount; i++) //add rest of enemies to list
        {
            //set up chance list
            enemyTypes.Clear();

            for (int j = 0; j < 3; j++) //common enemies
            { enemyTypes.Add("common"); }

            if (!(preset.mediumRarityEnemies.Count == 0)){ //medium enemies
                enemyTypes.Add("medium");
                enemyTypes.Add("medium");}

            if (!(preset.RareEnemies.Count == 0)){ //rare enemies
                enemyTypes.Add("rare");}

            if (!(preset.SwarmEnemies.Count == 0)){ //swarm enemies
                enemyTypesCount = enemyTypes.Count;
                for (int j = 0; j < enemyTypesCount; j++) //always 1 in 2 chance
                { enemyTypes.Add("swarm"); }
            }

            enemyTypeToSpawn = enemyTypes[Random.Range(0, enemyTypes.Count)]; //pick enemy type

            //add enemy type to list
            if(enemyTypeToSpawn == "common")
            {
                enemiesToSpawn.Add(enemy_library[preset.basicEnemies[Random.Range(0,preset.basicEnemies.Count)]].prefab);
            }else if (enemyTypeToSpawn == "medium")
            {
                enemiesToSpawn.Add(enemy_library[preset.mediumRarityEnemies[Random.Range(0, preset.mediumRarityEnemies.Count)]].prefab);
            }
            else if (enemyTypeToSpawn == "rare")
            {
                enemiesToSpawn.Add(enemy_library[preset.RareEnemies[Random.Range(0, preset.RareEnemies.Count)]].prefab);
            }
            else if (enemyTypeToSpawn == "swarm")
            {
                enemy_number--;
                swarmToSpawn = Random.Range(preset.swarmMultiplierMin, preset.swarmMultiplierMax + 1);
                for (int j = 0; j < swarmToSpawn; j++)
                { 
                    enemiesToSpawn.Add(enemy_library[preset.SwarmEnemies[Random.Range(0, preset.SwarmEnemies.Count)]].prefab);
                    enemy_number++;
                }
            }
            //keep looping until enough enemies spawned
        }

            room_script.enemy_count = enemy_number; //set the number of enemies in the room script

        for(int i = 0; i < enemiesToSpawn.Count; i++)
        {
            //instantiate enemies
            GameObject instantiated_enemy = Instantiate(enemiesToSpawn[i], new Vector2(room_centre.x + Random.Range(left_x_offset,right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset)), Quaternion.identity);

            //set values in instantiated enemy
            instantiated_enemy.GetComponent<enemyHealth>().room_index = room_script.room_index;

            while (Physics2D.OverlapCircle(instantiated_enemy.transform.position,instantiated_enemy.GetComponent<enemyHealth>().spawn_check_radius, environment)) //check the enemies are not in walls or by the doors
                instantiated_enemy.transform.position = new Vector2(room_centre.x + Random.Range(left_x_offset, right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset));
        }
    }

    public void spawn_boss_room(room_logic room_script, Vector3 room_centre)
    {
        int boss_number = bossPool[Random.Range(0, bossPool.Count)]; //pick boss

        room_script.enemy_count = 1; //set enemy count to 1
        
        //instantiate enemies
        GameObject instantiated_enemy = Instantiate(enemy_library[boss_number].prefab, new Vector2(room_centre.x + Random.Range(left_x_offset, right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset)), Quaternion.identity);

        //set values in instantiated enemy
        instantiated_enemy.GetComponent<enemyHealth>().room_index = room_script.room_index;

        while (Physics2D.OverlapCircle(instantiated_enemy.transform.position, instantiated_enemy.GetComponent<enemyHealth>().spawn_check_radius, environment)) //check the enemies are not in walls or by the doors
            instantiated_enemy.transform.position = new Vector2(room_centre.x + Random.Range(left_x_offset, right_x_offset), room_centre.y + Random.Range(lower_y_offset, upper_y_offset));
    }
}

