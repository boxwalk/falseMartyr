using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modificationLogic : MonoBehaviour
{
    //main values
    private string parent;

    //components
    private SpriteRenderer rend;
    private SpriteRenderer target_rend;

    //references
    private ReferenceController reference;
    private itemController item;
    private modificationController mod;

    void Start()
    {
        //get components
        rend = GetComponent<SpriteRenderer>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        item = reference.ItemController;
        mod = reference.ModificationController;

        //find parent
        parent = item.item_library[mod.modification_list[mod.modification_list.Count-1]].mod_parent;

        //get target renderer
        if (parent == "body") //parent is body so transform.parent will give a valid game object
        {
            target_rend = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        rend.material = target_rend.material; //set material
    }
}
