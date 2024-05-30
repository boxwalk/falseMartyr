using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class roomConfigController : MonoBehaviour
{
    [Serializable]
    public class roomConfig //custom class
    {
        public GameObject configPrefab;
        public int weight;
    }

    public List<roomConfig> roomConfigibrary;

    private List<GameObject> roomConfigtemp = new();

    void Start()
    {
        //setup temp list
        for(int i = 0; i < roomConfigibrary.Count; i++)
        {
            for (int j = 0; j < roomConfigibrary[i].weight; j++)
            {
                roomConfigtemp.Add(roomConfigibrary[i].configPrefab);
            }
        }
    }

    public void getRoomConfig(Transform targetRoom)
    {
        Instantiate(roomConfigtemp[Random.Range(0,roomConfigtemp.Count)], targetRoom); //instatiate random config prefab
    }
}
