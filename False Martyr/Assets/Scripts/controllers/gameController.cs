using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    [Header("main menu")]
    [SerializeField] private Animator menuAnimator;

    //do not destroy on load
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void new_run_button_press()
    {
        menuAnimator.SetTrigger("characterMenu");
    }
}
