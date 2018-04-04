﻿using Mission;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingBarController : MonoBehaviour {
    public GameObject startMenuObject;
    public Slider loadingBar;
    int num;
    Scene scene;
    
    public void Loading()
    {
        //Debug.Log("Loading Mission.. Hold On Tight..  ");

        if(MissionLifeCycleController.Initialized == true && loadingBar.value <= 10999)
        {
            num = Random.Range(0,7);
            loadingBar.value += num;
            //Debug.Log("Loading Mission.. Hold On Tight..  " + loadingBar.value);
        }

        if (MissionLifeCycleController.Initialized == false && loadingBar.value <= 5000)
        {
            num = Random.Range(0, 7);
            loadingBar.value += num;
            Debug.Log("Have Not Initialized Successfully" + loadingBar.value);
        }

        //if (scene.name == "MissionScene" && MissionLifeCycleController.Initialized == true && loadingBar.value <= 1000)
        //{
        //    loadingBar.value = 5000;
        //}
    }

    private void OnEnable()
    {
        loadingBar.value = 0;
    }

    // Use this for initialization
    void Start () {
        loadingBar.value = 0;
        loadingBar.maxValue = 11000;
        Loading();
	}

    // Update is called once per frame
    void Update()
    {
        Loading();
    }
}
