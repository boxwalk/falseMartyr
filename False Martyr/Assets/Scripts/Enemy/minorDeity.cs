using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class minorDeity : enemyAbstract
{
    //main values
    private float attack_ready;
    private int attack_number = 0;
    private bool former_activated = false;
    private bool touchingWall = false;
    private bool full_activation = false;

    //serialized values
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<int> attack_selection;
    [SerializeField] private float bulletHellSpeed;
    [SerializeField] private float bulletHellWait;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float minAttackReadyTime;
    [SerializeField] private float maxAttackReadyTime;
    [SerializeField] private int scythesToShoot;
    [SerializeField] private float scythesCooldown;
    [SerializeField] private List<Transform> scythePoints;

    //prefabs
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject scythe;
    [SerializeField] private GameObject teleportParticles;
    [SerializeField] private GameObject dashParticles; 

    //components
    private Animator anim;
    private SpinningAnimation spin;
    private Transform body_transform;
    private Rigidbody2D rb;
    private CheckCollisionWithTag up_check;
    private CheckCollisionWithTag down_check;
    private CheckCollisionWithTag left_check;
    private CheckCollisionWithTag right_check;

    //references
    private GameObject player;
    private room_controller room;
    private UIController UIController;

    //events 
    [HideInInspector] public UnityEvent death;

    void Start()
    {
        start_logic(); //method from derived class

        //get components
        anim = GetComponent<Animator>();
        body_transform = transform.GetChild(0).GetChild(0);
        spin = body_transform.GetComponent<SpinningAnimation>();
        rb = GetComponent<Rigidbody2D>();
        up_check = transform.GetChild(1).GetChild(3).GetComponent<CheckCollisionWithTag>();
        down_check = transform.GetChild(1).GetChild(4).GetComponent<CheckCollisionWithTag>();
        left_check = transform.GetChild(1).GetChild(1).GetComponent<CheckCollisionWithTag>();
        right_check = transform.GetChild(1).GetChild(2).GetComponent<CheckCollisionWithTag>();

        //get references
        player = reference.Player;
        room = reference.RoomController;
        UIController = reference.UIController;
    }

    void Update()
    {
        update_logic(); //method from derived class
        if (enemy_activated)
        {
            if (!former_activated){ //run logic on first activation
                former_activated = true;
                StartCoroutine(firstActivationLogic());}
            if(full_activation)
                enemy_behaviour(); //run enemy logic if activated
        }
    }

    void enemy_behaviour()
    {
        if(attack_number == 0 && Time.time > attack_ready) //attack charged
        {
            attack_number = attack_selection[Random.Range(0,attack_selection.Count)]; //execute attack
            rb.velocity = new Vector2(0, 0); //set velocity to 0
            anim.SetInteger("AttackType", attack_number);
            anim.SetTrigger("newAttack");
            if(attack_number == 1)
                StartCoroutine(spinAttack()); //attack 1
            else if(attack_number == 2)
                StartCoroutine(bulletHellAttack());//attack 2
            else if (attack_number == 3)
                StartCoroutine(dashAttack());//attack 3
            else if (attack_number == 4)
                StartCoroutine(scytheAttack());//attack 4
        }
        else if(attack_number == 0)
        {
            //enemy movement
            transform.right = player.transform.position - transform.position; //"looks" at player
            rb.velocity = (transform.right).normalized * moveSpeed; //move towards player
            transform.rotation = Quaternion.identity; //reset rotation
        }

        if (!spin.enabled) //reset y rotation
        {
            body_transform.rotation = Quaternion.identity;
        }
    }

    private IEnumerator firstActivationLogic()
    {
        //initiialize values
        attack_ready = Time.time + Random.Range(minAttackReadyTime, maxAttackReadyTime);
        //set trigger
        UIController.anim.SetTrigger("bossScreen");

        yield return new WaitForSeconds(1.3f);
        full_activation = true;
    }

    private IEnumerator spinAttack() //spin and then launch bullets in all directions
    {
        yield return new WaitForSeconds(2); //wait for animation

        GameObject bullet;
        List<float> dirs = new List<float> {90,180,270,0, 45,135,225,315, 22,67,112,157,202,247,292,337}; //get directions
        foreach(float dir in dirs) //instantiate bullet in each direction
        { 
            bullet = Instantiate(projectile, transform.position, Quaternion.identity);
            enemyProjectile bulletScript = bullet.GetComponent<enemyProjectile>();
            bulletScript.dir = dir; //set direction of bullet
            death.AddListener(bulletScript.destroy_bullet); //make sure bullet will destroy on death
        }

        re_ready_attack(); //attack complete
    }

    private void re_ready_attack() //reset attack
    {
        attack_ready = Time.time + Random.Range(minAttackReadyTime, maxAttackReadyTime);
        attack_number = 0;
    }

    private IEnumerator bulletHellAttack() //spin and then launch bullets in all directions
    {
        yield return new WaitForSeconds(2); //wait for animation

        transform.position = room.GetPositionFromGridIndex(cam.room_index); //teleport to middle of room
        Instantiate(teleportParticles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);

        GameObject bullet;
        int safeZone;
        for(int j= 0; j <3; j++)
        {
            safeZone = Random.Range(0, 36);
            for (int i = 0; i < 36; i++) //instantiate bullet in each direction
            {
                if (Mathf.Abs(i - safeZone) > 2) //checks if the bullet needs to be fired. eg if safezone is 10 bullets 8,9,10,11 and 12 would not be fired
                {
                    bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                    enemyProjectile bulletScript = bullet.GetComponent<enemyProjectile>();
                    bulletScript.dir = i * 10; //set direction of bullet
                    bulletScript.bullet_speed = bulletHellSpeed;
                    bulletScript.StartCoroutine(bulletScript.up_speed(bulletHellWait));
                    death.AddListener(bulletScript.destroy_bullet); //make sure bullet will destroy on death
                }
            }

            yield return new WaitForSeconds(bulletHellWait + 2); //wait
        }
        re_ready_attack(); //attack complete

        //removes attack from list ; therefore this attack is limited to occuring twice per fight
        if (attack_selection.Count == 12)
            attack_selection.RemoveAt(11);
        else if (attack_selection.Count == 11)
            attack_selection.RemoveAt(10);
    }

    private IEnumerator dashAttack() //spin and then launch bullets in all directions
    {
        yield return new WaitForSeconds(1); //give chance for dodge

        //get dash direction
        string dash_dir = "none"; //initialize dash direction
        if(up_check.isTouching) //check whether player is in a certain direction
            dash_dir = "up";
        else if (down_check.isTouching)
            dash_dir = "down";
        else if (left_check.isTouching)
            dash_dir = "left";
        else if (right_check.isTouching)
            dash_dir = "right";
        if(dash_dir == "none") //dash in general direction of player
        {
            int UpDownOrLeftRight = Random.Range(0, 2); //0 is up/down, 1 is left/right
            if(UpDownOrLeftRight == 0)
            {
                if(transform.position.y > player.transform.position.y) //above player
                    dash_dir = "down";
                else //below player
                    dash_dir = "up";
            }
            else
            {
                if (transform.position.x > player.transform.position.x) //right of player
                    dash_dir = "left";
                else //left of player
                    dash_dir = "right";
            }
        }

        //dash in direction
        if (dash_dir == "up")
            rb.velocity = new Vector2(0, dashSpeed);
        else if (dash_dir == "down")
            rb.velocity = new Vector2(0, -dashSpeed);
        else if (dash_dir == "left")
            rb.velocity = new Vector2(-dashSpeed, 0);
        else
            rb.velocity = new Vector2(dashSpeed, 0);

        //create dash particles
        GameObject _dashParticles = Instantiate(dashParticles, transform.position, Quaternion.identity, transform);

        while (!touchingWall) //wait until dash hits wall
        {
            yield return null;
        }
        _dashParticles.GetComponent<ParticleSystem>().Stop();
        StartCoroutine(destroyDashParticles(_dashParticles));
        re_ready_attack(); //attack complete
        anim.SetTrigger("dashComplete");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            touchingWall = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            touchingWall = false;
    }
    private IEnumerator destroyDashParticles(GameObject _dashParticles)
    {
        yield return new WaitForSeconds(3);
        Destroy(_dashParticles); //destroy particles
    }


    private IEnumerator scytheAttack() //spin and then launch bullets in all directions
    {
        yield return new WaitForSeconds(2); //wait for animation

        List<Transform> shootPoints = new(); //pick shoot points
        List<Transform> tempList = new();
        foreach (Transform item in scythePoints) //initialize temporary list
            tempList.Add(item);
        int removeIndex;
        for(int i = 0; i < scythesToShoot; i++)
        {
            removeIndex = Random.Range(0, tempList.Count);
            shootPoints.Add(tempList[removeIndex]);
            tempList.RemoveAt(removeIndex);
        }

        GameObject _scythe;
        foreach(Transform pos in shootPoints) //instatntiate scythes
        {
            _scythe = Instantiate(scythe, pos.position, Quaternion.identity);
            death.AddListener(_scythe.GetComponent<scytheProjectile>().destroy_bullet); //make sure bullet will destroy on death
            yield return new WaitForSeconds(scythesCooldown);
        }

        anim.SetTrigger("scyhteComplete");
        re_ready_attack(); //attack complete
    }
}