using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    [Header("main menu")]
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private Animator storyScreenAnimator;

    //main menu
    [HideInInspector] public Coroutine storyScreenCoroutine;
    [HideInInspector] public Coroutine skipCoroutine;

    //do not destroy on load
    void Awake()
    {
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
        startFinalLoad();
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
        startFinalLoad();
    }

    public void startFinalLoad()
    {
        menuAnimator.SetTrigger("finalLoad");
    }
}
