using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodRespawner : MonoBehaviour
{
    private float time;
    public float delay = 5;
    public bool startTimer;
    public GameObject food;
    public FlockManager FlockManager;
    public FoodController Controller;
    private GameObject currentFood;

    private void Start()
    {
        FlockManager = GetComponentInChildren<FishFood>().flockType;
        currentFood = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (currentFood.GetComponentInChildren<FishFood>().hasBeenReleased)
        {
            startTimer = true;
        }
        if (startTimer)
        {
            time += Time.deltaTime;
            if (time >= delay)
            {
                InstantiateNewFood();
            }
        }
    }


    public void InstantiateNewFood()
    {
        time = 0;
        startTimer = false;
        GameObject newFood = Instantiate(food, transform);
        FishFood newFishFood = newFood.GetComponentInChildren<FishFood>();
        newFishFood.flockType = FlockManager;
        newFishFood.controller = Controller;
        currentFood = newFood;
    }
}
