using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    //Serialized movement values
    [SerializeField] private float speed;

    //Input values
    private bool left_key_pressed;
    private bool right_key_pressed;
    private bool up_key_pressed;
    private bool down_key_pressed;

    //Components
    private Rigidbody2D rb;

    //references
    private ReferenceController reference;
    private gameplayController gameplayController;

    //movement values
    private float finalSpeed;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        gameplayController = reference.GameplayController;
    }

    //Update to run subroutines
    void Update()
    {
        playerMovementController();
    }

    //everything contianed in the update method
    void playerMovementController()
    {
        speed_calculation();
        gather_input();
        playerMovement();
    }

    //player movement 
    void playerMovement()
    {
        //horizontal movement
        float x_velocity = 0f;
        if (left_key_pressed)
        {x_velocity = -1f;}
        else if (right_key_pressed)
        {x_velocity = 1f;}

        //vertical movement
        float y_velocity = 0f;
        if (up_key_pressed)
        {y_velocity = 1f; }
        else if (down_key_pressed)
        {y_velocity = -1f; }

        //normalize vector
        Vector2 movement_dir = new Vector2(x_velocity, y_velocity).normalized;

        //set to rigidbody velocity
        rb.velocity = (movement_dir * finalSpeed);
    }

    //gather input
    void gather_input()
    {
        //directional inputs
        left_key_pressed = Input.GetKey(KeyCode.A);
        right_key_pressed = Input.GetKey(KeyCode.D);
        up_key_pressed = Input.GetKey(KeyCode.W);
        down_key_pressed = Input.GetKey(KeyCode.S);
    }

    void speed_calculation()
    {
        if (gameplayController.enemy_room_clear)
            finalSpeed = speed;
        else
            finalSpeed = speed * 0.8f;
    }
}
