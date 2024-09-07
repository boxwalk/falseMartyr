using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class player_coins : MonoBehaviour
{
    //main values
    public int coins = 0;

    //ui references
    [SerializeField] private GameObject coin_counter;
    [SerializeField] private TextMeshProUGUI text;

    //components
    [SerializeField] private Image healthBar;

    //transforms
    public List<Transform> coin_points;

    //prefabs
    [SerializeField] private GameObject coinPrefab;
    public GameObject greedBonus;


    //references
    private ReferenceController ReferenceController;
    private statController stats;
    private gameplayController gameplay;

    void Start()
    {
        ReferenceController = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = ReferenceController.StatController;
        gameplay = ReferenceController.GameplayController;
    }

    void Update()
    {
        text.text = coins.ToString(); //set coin counter display

        if (coins == 0 || healthBar.enabled) //does not show coin counter if on 0 coins 
            coin_counter.SetActive(false);
        else
            coin_counter.SetActive(true);

        if(coins < 0) //check for limit break
            coins = 0;
    }

    public void gain_coins(int amount, int greed_count) //get money
    {
        coins += amount;
        int greed_chance = Random.Range(1, 101);
        if(greed_chance < stats.greed_extra_coin_chance && greed_count < 5)
        {
            //greed proc
            List<Transform> temp_coin_transform = new();
            foreach (Transform point in coin_points)
                temp_coin_transform.Add(point);

            int chosen_point = 0;
            while(temp_coin_transform.Count> 0)
            {
                chosen_point = Random.Range(0, temp_coin_transform.Count);
                if(!Physics2D.OverlapCircle(temp_coin_transform[chosen_point].position, 0.4f, gameplay.reward_mask))
                    break;
                else
                    temp_coin_transform.RemoveAt(chosen_point);
            }

            GameObject new_coin;
            if(temp_coin_transform.Count == 0)
                new_coin = Instantiate(coinPrefab, transform.position, Quaternion.identity); //spawn pickup on player
            else
                new_coin = Instantiate(coinPrefab, temp_coin_transform[chosen_point].position, Quaternion.identity); //spawn pickup on point
            new_coin.GetComponent<pickupLogic>().greed_count = greed_count + 1;
            Instantiate(greedBonus, transform.position,Quaternion.identity); //particle
        }

    }
}
