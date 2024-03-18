using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modificationController : MonoBehaviour
{
    //main values
    public List<int> modification_list = new();

    //references
    private ReferenceController reference;
    private itemController item;
    private GameObject player;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        item = reference.ItemController;
        player = reference.Player;
    }

    public void create_modification(int itemIndex) //create a modification
    {
        if (!modification_list.Contains(itemIndex))
        {
            modification_list.Add(itemIndex); // add mod to list
            Transform parent = null;
            string parent_name = item.item_library[itemIndex].mod_parent;
            if (parent_name == "body") //get modification parent
            {
                parent = player.transform.GetChild(0).GetChild(0);
            }
            Instantiate(item.item_library[itemIndex].modification_prefab, parent); //instantiate modification
        }
    }
}
