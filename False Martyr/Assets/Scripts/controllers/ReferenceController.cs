using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceController : MonoBehaviour
{
    //reference controller gathers references then allows other scripts to access them

    //public references
    [HideInInspector] public camera_controller CameraController;
    [HideInInspector] public room_controller RoomController;
    [HideInInspector] public enemyGenController enemyGenController;
    [HideInInspector] public GameObject Player;
    [HideInInspector] public gameplayController GameplayController;
    [HideInInspector] public modificationController ModificationController;
    [HideInInspector] public itemController ItemController;
    [HideInInspector] public UIController UIController;
    [HideInInspector] public shopController ShopController;

    //assigns references to variables
    void Start()
    {
        CameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<camera_controller>();
        RoomController = GameObject.FindGameObjectWithTag("RoomController").GetComponent<room_controller>();
        enemyGenController = GameObject.FindGameObjectWithTag("EnemyGenController").GetComponent<enemyGenController>();
        Player = GameObject.FindGameObjectWithTag("Player");
        GameplayController = GameObject.FindGameObjectWithTag("GameplayController").GetComponent<gameplayController>();
        ItemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<itemController>();
        ModificationController = GameObject.FindGameObjectWithTag("ModController").GetComponent<modificationController>();
        UIController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        ShopController = GameObject.FindGameObjectWithTag("ShopController").GetComponent<shopController>();
    }
}
