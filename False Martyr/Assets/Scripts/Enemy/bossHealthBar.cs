using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class bossHealthBar : MonoBehaviour
{
    //gameobjects
    [SerializeField] private Image fill;
    [SerializeField] private Image background;
    [SerializeField] private Image ease;
    [SerializeField] private TextMeshProUGUI boss_title;

    //references
    private ReferenceController reference;
    private gameplayController gameplay;

    //values
    public float health;
    public float maxHealth;
    [SerializeField] private float lerpSpeed;
    private bool former_shown = false;

    //components
    private Slider slider;
    private Slider Easeslider;

    void Start()
    {
        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        gameplay = reference.GameplayController;

        //get components
        slider = transform.GetChild(1).GetComponent<Slider>();
        Easeslider = transform.GetChild(0).GetComponent<Slider>();
    }

    void Update()
    {
        if(gameplay.currentRoomType == "boss" && !gameplay.enemy_room_clear)
        {
            //show health bar
            fill.enabled = true;
            background.enabled = true;
            ease.enabled = true;
            boss_title.enabled = true;

            //set slider values
            slider.maxValue = maxHealth;
            Easeslider.maxValue = maxHealth;
            slider.value = health;
            Easeslider.value = Mathf.Lerp(Easeslider.value, health, lerpSpeed);

            former_shown = true;
        }else
        {
            if (former_shown == true)
            {
                StartCoroutine(hide_healthbar());
            }
            former_shown = false;
        }
    }

    private IEnumerator hide_healthbar()
    {
        float targettime = Time.time + 2;
        while(targettime > Time.time)
        {
            Easeslider.value = Mathf.Lerp(Easeslider.value, 0f, lerpSpeed);
            yield return null;
        }
        //hide health bar
        fill.enabled = false;
        background.enabled = false;
        ease.enabled = false;
        boss_title.enabled = false;
    }
}
