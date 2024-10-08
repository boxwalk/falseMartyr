using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack_logic : MonoBehaviour
{
    //public variables
    public int attack_dir = 1;

    //serialized values
    private float bullet_speed;
    private float damage;
    [SerializeField] private GameObject explosion_particle;
    [SerializeField] private GameObject damage_particle;

    //components
    private Rigidbody2D rb;
    private Animator anim;
    private CircleCollider2D _collider;

    //reference
    private ReferenceController reference;
    private statController stats;
    private SpriteRenderer rend;
    private player_attacks playerattacks;

    //sprites
    [SerializeField] private Sprite gaintsBloodSprite;
    [SerializeField] private Sprite fractureSprite;
    [SerializeField] private Sprite miniFractureSprite;
    [SerializeField] private Sprite scytheSprite;

    //main values
    private float baseSize;
    private float range;
    private float initializedTime;
    private bool rangeAnimStarted = false;
    private float sinWaveOffset;
    private float root_x;
    private float root_y;
    private float startTime;
    [SerializeField] private float wormWaveMagnitude;
    [SerializeField] private float wormWaveSpeed;
    [HideInInspector] public bool isTwinMaskSynegy = false;
    [HideInInspector] public bool isBoneFractureOffshoot = false;
    [HideInInspector] public float ovverideDir = Mathf.PI * 10000;
    [SerializeField] private float boneFractureSpread;
    [SerializeField] private Vector3 fractureSpriteOffset;
    [SerializeField] private Color giantsBloodBoneSynergy;
    [SerializeField] private GameObject fracture_particle;
    private bool RangeDestroy = false;
    private bool CameraDestroy = false;
    [SerializeField] private GameObject bracerSigil;
    [SerializeField] private Vector3 scytheSpriteOffset;
    private bool logic_enabled = true;

    void Start()
    {
        //get references to components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        _collider = GetComponent<CircleCollider2D>();
        _collider.enabled = false;

        //get references
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        stats = reference.StatController;
        rend = transform.GetChild(0).GetComponent<SpriteRenderer>();
        playerattacks = reference.Player.GetComponent<player_attacks>();

        //set up characteristics
        if (isBoneFractureOffshoot)
        {
            damage = stats.final_damage / 2;
            range = stats.range * 0.8f;
            bullet_speed = stats.bulletSpeed * 0.8f;
            baseSize = transform.localScale.x * 0.8f;
            explosion_particle = fracture_particle;
        }
        else
        {
            damage = stats.final_damage;
            range = stats.range;
            bullet_speed = stats.bulletSpeed;
            baseSize = transform.localScale.x;
        }
        float newScale = baseSize * (stats.bulletSize / 10);
        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
        initializedTime = Time.time;

        //pick sprite
        if (isBoneFractureOffshoot)
        {
            rend.sprite = miniFractureSprite; //mini fracture
            rend.gameObject.transform.localPosition += fractureSpriteOffset;
            rend.gameObject.GetComponent<SpinningAnimation>().enabled = true;
        }
        else if (stats.passiveItemEffects.Contains("scythe"))
        {
            rend.sprite = scytheSprite; //scythe
            rend.gameObject.GetComponent<SpinningAnimation>().enabled = true;
            rend.gameObject.transform.localScale = new Vector2(0.8f, 0.8f); //resize
            rend.gameObject.transform.localPosition += scytheSpriteOffset;
            logic_enabled = false;
            StartCoroutine(scytheCooldown());
            initializedTime += 3;
            anim.SetLayerWeight(1, 1);
        }
        else if (stats.passiveItemEffects.Contains("fracture"))
        {
            rend.sprite = fractureSprite; //bone fracture
            rend.gameObject.transform.localPosition += fractureSpriteOffset;
            rend.gameObject.GetComponent<SpinningAnimation>().enabled = true;
            if (stats.passiveItemEffects.Contains("giantsBlood"))
                rend.color = giantsBloodBoneSynergy;
        }
        else if (stats.passiveItemEffects.Contains("giantsBlood"))
            rend.sprite = gaintsBloodSprite; // giant's blood item effect

        //setup variables
        root_x = transform.position.x;
        root_y = transform.position.y;
        startTime = Time.time;

        if(ovverideDir == Mathf.PI * 10000) //checks if it is a normal bullet
        {
            //set direction of bullet (right is automatically done)
            if (attack_dir == 2)
                transform.right = -transform.right; //attack left
            else if (attack_dir == 3)
                transform.right = transform.up; //attack up
            else if (attack_dir == 4)
                transform.right = -transform.up; //attack down
        }
        else
        {
            transform.Rotate(new Vector3(0f, 0f, ovverideDir)); //unusual bullet
        }
    }

    void Update()
    {
        if (logic_enabled)
        {
            //enable hitbox
            _collider.enabled = true;

            //set velocity of bullet
            rb.velocity = transform.right * bullet_speed;

            rangeLogic();

            //specific item logic
            if (stats.passiveItemEffects.Contains("worm") && !isBoneFractureOffshoot)
                wigglyWormLogic();
        }
        else
        {
            _collider.enabled = false;
        }
    }

    void destroy_bullet()
    {
        //specific item logic
        if (stats.passiveItemEffects.Contains("fracture") && !isBoneFractureOffshoot && !RangeDestroy && !CameraDestroy)
            boneFractureLogic();
        if (stats.passiveItemEffects.Contains("bracers") && !isBoneFractureOffshoot && !CameraDestroy)
            sigilBracersLogic();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isBoneFractureOffshoot || (isBoneFractureOffshoot && startTime + 0.1f < Time.time))
        if (collision.gameObject.layer == 6 && !(collision.gameObject.tag == "NoEnemyField")){  //hit walls
            Instantiate(explosion_particle, transform.position, Quaternion.identity); //instantiate particles
            destroy_bullet();}
        else if (collision.gameObject.layer == 7)
        {
            if (stats.passiveItemEffects.Contains("divergence")) //divergence special logic
            {
                enemyHealth enemyHealthScript = collision.gameObject.GetComponent<enemyHealth>();
                enemyHealthScript.enemy_health -= (damage/10); //hit enemies for 10 percent damage
                    enemyHealthScript.StartCoroutine(enemyHealthScript.divergenceDamage(damage * 1.5f)); //start coroutine for 150 percent damage
            }
            else
            {
                collision.gameObject.GetComponent<enemyHealth>().enemy_health -= damage; //hit enemies
            }
            Instantiate(damage_particle, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<enemyHealth>().damage_flash(); // damage anim
            destroy_bullet(); 
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera" && logic_enabled)
        {
            CameraDestroy = true;
            destroy_bullet(); //left camera
        }
    }
    
    void rangeLogic()
    {
        if(((Time.time - initializedTime + 0.5f)*bullet_speed) > range && !rangeAnimStarted)
        {
            //start playing range anim
            anim.SetTrigger("rangeOut");
            rangeAnimStarted = true;
        }
        else if (((Time.time - initializedTime) * bullet_speed) > range)
        {
            RangeDestroy = true;
            destroy_bullet();
        }
    }

    void wigglyWormLogic()
    {
        sinWaveOffset = Mathf.Sin((Time.time - startTime) * wormWaveSpeed) * wormWaveMagnitude;
        if (isTwinMaskSynegy) //twin mask synergy
            sinWaveOffset = -sinWaveOffset;
        if (attack_dir < 3)
        {
            //horizontal so wave on vertical
            transform.position = new Vector2(transform.position.x, root_y + sinWaveOffset);
        }
        else
        {
            //vertical so wave on horizontal
            transform.position = new Vector2(root_x + sinWaveOffset, transform.position.y);
        }
    }

    void boneFractureLogic()
    {
        float firstBoneDir = 0;
        float secondBoneDir = 0;
        if(attack_dir == 2)
        {
            firstBoneDir = boneFractureSpread;
            secondBoneDir = -boneFractureSpread;
        }
        else if (attack_dir == 1)
        {
            firstBoneDir = 180 + boneFractureSpread;
            secondBoneDir = 180 - boneFractureSpread;
        }
        else if (attack_dir == 4)
        {
            firstBoneDir = 90 + boneFractureSpread;
            secondBoneDir = 90 - boneFractureSpread;
        }
        else if (attack_dir == 3)
        {
            firstBoneDir = 270 + boneFractureSpread;
            secondBoneDir = 270 - boneFractureSpread;
        }

        GameObject instantiatedBone = Instantiate(playerattacks.player_attack,transform.position, Quaternion.identity);
        attack_logic boneLogic = instantiatedBone.GetComponent<attack_logic>();
        boneLogic.isBoneFractureOffshoot = true;
        boneLogic.ovverideDir = firstBoneDir;
        instantiatedBone = Instantiate(playerattacks.player_attack, transform.position, Quaternion.identity);
        boneLogic = instantiatedBone.GetComponent<attack_logic>();
        boneLogic.isBoneFractureOffshoot = true;
        boneLogic.ovverideDir = secondBoneDir;
    }
    void sigilBracersLogic()
    {
        bool is_touching_other_sigil = false;
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, 0.7f * (stats.bulletSize / 10));
        foreach (Collider2D col in results)
        {
            if (col.gameObject.tag == "bracerSigil")
            {
                is_touching_other_sigil = true;
                break;
            }
        }
        if(!is_touching_other_sigil)
            Instantiate(bracerSigil, transform.position, Quaternion.identity);
    }

    private IEnumerator scytheCooldown()
    {
        yield return new WaitForSeconds(2.34f);
        if (stats.passiveItemEffects.Contains("giantsBlood"))
            rend.color = giantsBloodBoneSynergy;
        yield return new WaitForSeconds(0.66f);
        logic_enabled = true;
    }
}
