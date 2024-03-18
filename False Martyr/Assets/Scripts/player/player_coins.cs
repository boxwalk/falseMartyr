using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class player_coins : MonoBehaviour
{
    //main values
    public int coins = 0;

    //ui references
    [SerializeField] private GameObject coin_counter;
    [SerializeField] private TextMeshProUGUI text;

    void Update()
    {
        text.text = coins.ToString(); //set coin counter display

        if (coins == 0) //does not show coin counter if on 0 coins 
            coin_counter.SetActive(false);
        else
            coin_counter.SetActive(true);

        if(coins < 0) //check for limit break
            coins = 0;
    }

    public void gain_coins(int amount) //get money
    {
        coins += amount;
    }
}
