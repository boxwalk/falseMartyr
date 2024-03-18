using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    //components
    private Animator anim;

    //children
    private TextMeshProUGUI Top_text;
    private TextMeshProUGUI Bottom_text;

    void Start()
    {
        anim = GetComponent<Animator>(); //get components
        Top_text = transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>(); //get children
        Bottom_text = transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void flavourText(string top_text, string bottom_text) //flavour text bar on item pickup
    {
        Top_text.text = top_text; //set text
        Bottom_text.text = bottom_text;
        anim.SetTrigger("bar_in"); //flavour text bar anim
    }
}
