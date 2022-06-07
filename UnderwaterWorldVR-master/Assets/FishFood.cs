using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFood : MonoBehaviour
{
    public int ID;
    public FlockManager flockType;
    public Vector3 foodPos;
    public float foodLeft;
    private int numFishEating;
    private string fishname;
    public OVRGrabbable _grabbable;
    private bool hasBeenGrabbed;
    public bool hasBeenReleased;
    public Rigidbody rb;
    public FoodController controller;
    private bool doOnlyOnce = true;
    private float startingFoodAmount;
    private float time;
    private float delay = 3;
    

    private void Start()
    {
        fishname = flockType.fishPrefab.name;
        fishname += "(Clone)";
        foodLeft = flockType.numFish * 1000;
        startingFoodAmount = foodLeft;
        StartCoroutine(SetColor());
    }

    public IEnumerator SetColor()
    {
        yield return new WaitForEndOfFrame();
        
        var thisRenderer = GetComponentInParent<Renderer>();
        var fishRenderer = flockType.fishPrefab.GetComponent<Renderer>();
        var fishColor = fishRenderer.sharedMaterial.GetColor("_Color");
        thisRenderer.material.SetColor("_BaseColor", fishColor);
    }

    private void FixedUpdate()
    {
        if (_grabbable.grabbedBy != null)
        {
            hasBeenGrabbed = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
        }

        if (hasBeenGrabbed && _grabbable.grabbedBy == null && doOnlyOnce)
        {
            hasBeenReleased = true;
            time += Time.deltaTime;
            if (time >= delay)
            {
                controller.StartSpawningNewFood(ID);
                doOnlyOnce = false;
                
            }
        }
        
        
        foodPos = this.transform.position;
        if (numFishEating > 0 && hasBeenReleased)
        {
            DecreaseFoodAmount();
        }
    }

    public void DecreaseFoodAmount()
    {
        foodLeft -= numFishEating;
        if (foodLeft < 1)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == fishname && hasBeenReleased)
        {
            other.GetComponent<Flock>().food = this;
            numFishEating++;
            Debug.Log("Eating");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == fishname && hasBeenReleased)
        {
            numFishEating--;
            other.GetComponent<Flock>().food = null;
        }
    }
}
