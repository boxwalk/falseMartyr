using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public class flavourTextValues
    {
        public string top_text;
        public string bottom_text;
    }

    //components
    [HideInInspector] public Animator anim;

    //children
    private TextMeshProUGUI Top_text;
    private TextMeshProUGUI Bottom_text;

    //values
    private bool ready_for_flavour_text = true;
    private Queue<flavourTextValues> flavour_texts = new();

    void Start()
    {
        anim = GetComponent<Animator>(); //get components
        Top_text = transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>(); //get children
        Bottom_text = transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (ready_for_flavour_text && flavour_texts.Count > 0)
        {
            flavourTextValues values = flavour_texts.Dequeue();
            Top_text.text = values.top_text; //set text
            Bottom_text.text = values.bottom_text;
            anim.SetTrigger("bar_in"); //flavour text bar anim
            ready_for_flavour_text = false;
            StartCoroutine(ready_flavour_text());
        }
    }

    public void flavourText(string top_text, string bottom_text) //flavour text bar on item pickup
    {
        flavourTextValues values = new();
        {
            values.top_text = top_text;
            values.bottom_text = bottom_text;
        }
        flavour_texts.Enqueue(values);
    }

    private IEnumerator ready_flavour_text()
    {
        yield return new WaitForSeconds(4);
        ready_for_flavour_text = true;
    }
}
