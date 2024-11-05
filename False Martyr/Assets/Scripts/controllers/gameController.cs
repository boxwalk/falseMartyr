using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{
    [Header("main menu")]
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private Animator storyScreenAnimator;

    [Header("testing framework")]
    public bool testingForm;

    //main menu
    [HideInInspector] public Coroutine storyScreenCoroutine;
    [HideInInspector] public Coroutine skipCoroutine;

    //main game
    public float floor = 1;
    private ReferenceController reference;
    private room_controller room;
    private statController stats;
    private GameObject loadingScreen;
    [HideInInspector] public bool fullGameStart = false;
    private UIController ui;

    //do not destroy on load
    void Awake()
    {
        if(!testingForm)
            DontDestroyOnLoad(gameObject);
    }

    //button on main menu pressed
    public void new_run_button_press()
    {
        menuAnimator.SetTrigger("characterMenu");
    }

    //button on character screen pressed
    public void continue_button_press()
    {
        menuAnimator.SetTrigger("storyMenu");
        storyScreenCoroutine = StartCoroutine(storyScreen());
        skipCoroutine = StartCoroutine(skipButton());
    }

    //story screen logic
    private IEnumerator storyScreen()
    {
        yield return new WaitForSeconds(1);
        storyScreenAnimator.SetTrigger("start");
        yield return new WaitForSeconds(80);
        //end story screen
        StopCoroutine(skipCoroutine);
        StartCoroutine(startFinalLoad());
    }

    //skip button logic
    private IEnumerator skipButton()
    {
        yield return new WaitForSeconds(0.2f);
        while (!Input.anyKeyDown) //wait for any key pressed
            yield return null;
        storyScreenAnimator.SetTrigger("skip button pressed"); //prompt anim
        yield return new WaitForSeconds(0.2f);
        while (!Input.anyKeyDown) //wait for any key pressed
            yield return null;
        //end story screen
        StopCoroutine(storyScreenCoroutine);
        StartCoroutine(startFinalLoad());
    }

    public IEnumerator startFinalLoad()
    {
        if(floor == 1) //only on first load
        {
            menuAnimator.SetTrigger("finalLoad");
            yield return new WaitForSeconds(2f);
        }
        SceneManager.LoadScene("main_game");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //main game start
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        room = reference.RoomController;
        ui = reference.UIController;
        stats = reference.StatController;
        stats.room = room;
        loadingScreen = GameObject.FindGameObjectWithTag("loading");
        while (!room.full_gen_complete) //wait for generation
            yield return null;
        Destroy(loadingScreen); //begin game
        fullGameStart = true;
        //logic on game start
        ui.anim.SetTrigger("dungeontitle"); //set title screen
    }

    void Start() //only for testing form
    {
        if (testingForm)
            StartCoroutine(testingFormLogic());
    }

    public IEnumerator testingFormLogic() //logic only run in testing form
    {
        //main game start
        reference = GameObject.FindGameObjectWithTag("ReferenceController").GetComponent<ReferenceController>();
        room = reference.RoomController;
        ui = reference.UIController;
        stats = reference.StatController;
        stats.room = room;
        loadingScreen = GameObject.FindGameObjectWithTag("loading");
        while (!room.full_gen_complete) //wait for generation
            yield return null;
        Destroy(loadingScreen); //begin game
        fullGameStart = true;
        //logic on game start
        ui.anim.SetTrigger("dungeontitle"); //set title screen
    }
    public void nextFloor() //exit reached
    {
        if(floor == 2)
        {
            SceneManager.LoadScene(3); //load end screen
        }
        else
        {
            floor++; //next floor
            StartCoroutine(startFinalLoad());
        }
    }
}
