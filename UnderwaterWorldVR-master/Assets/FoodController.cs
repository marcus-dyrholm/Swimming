using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public FishFood[] fishFoods;

    public FoodRespawner[] respawners;
    // Start is called before the first frame update
    void Start()
    {
        fishFoods = GetComponentsInChildren<FishFood>();
        for (int i = 0; i < fishFoods.Length; i++)
        {
            fishFoods[i].ID = i;
        }
        respawners = GetComponentsInChildren<FoodRespawner>();
        for (int i = 0; i < respawners.Length; i++)
        {
            respawners[i].Controller = this;
        }
    }

    public void StartSpawningNewFood(int ID)
    {
        respawners[ID].startTimer = true;
    }
    


}
