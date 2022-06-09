using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockManager : MonoBehaviour {

    public GameObject fishPrefab;
    public int numFish = 20;
    public List<GameObject> allFish = new List<GameObject>();
    public Vector3 swimLimits = new Vector3(5.0f, 5.0f, 5.0f);
    public Vector3 goalPos;
    public Color gizmoColor;

    public int changeGoalPosChance = 10;

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(0.0f, 10.0f)]
    public float neighbourDistance;
    [Range(0.0f, 10.0f)]
    public float rotationSpeed;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(gizmoColor.r,gizmoColor.g,gizmoColor.b,0.2f);
        Gizmos.DrawCube(transform.position, swimLimits);
        Gizmos.color = new Color(gizmoColor.r,gizmoColor.g,gizmoColor.b,gizmoColor.a + 0.2f);
        Gizmos.DrawSphere(goalPos,0.2f);
    }

    void Start() {
        for (int i = 0; i < numFish; ++i) {
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-swimLimits.x/3, swimLimits.x/3),
                Random.Range(-swimLimits.y/3, swimLimits.y/3),
                Random.Range(-swimLimits.z/3, swimLimits.z)/3);
            GameObject newFish = (GameObject) Instantiate(fishPrefab, pos, Quaternion.identity);
            allFish.Add(newFish);
            allFish[i].GetComponent<Flock>().flockManager = this;
            
        }
        goalPos = this.transform.position;
        InvokeRepeating("CheckFoodLevels",10,10);
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        if (Random.Range(0,1000) < changeGoalPosChance)
        {
            goalPos = this.transform.position + new Vector3(
                Random.Range(-swimLimits.x/2, swimLimits.x/2),
                Random.Range(-swimLimits.y/2, swimLimits.y/2),
                Random.Range(-swimLimits.z/2, swimLimits.z/2));
        }
        
    }

    void CheckFoodLevels()
    {
        float totalFoodLevel = 0;
        for (int i = 0; i < allFish.Count; i++)
        {
            if (allFish[i] == null)
            {
                allFish.RemoveAt(i);
            }
            else
            {
                totalFoodLevel += allFish[i].GetComponent<Flock>().foodlevel;
            }

        }

        totalFoodLevel = totalFoodLevel / numFish;

        if (totalFoodLevel >= 75)
        {
            GameObject newFish = Instantiate(fishPrefab, transform.position, quaternion.identity);
            newFish.GetComponent<Flock>().flockManager = this;
            allFish.Add(newFish);
        }
    }
}
