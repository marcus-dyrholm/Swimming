using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flock : MonoBehaviour {

    public FlockManager flockManager;
    float speed;
    public int resetSpeedChance = 10;
    public int applyRulesChance = 20;

    public bool turning = false;
    public bool turningAwayFromPredator;
    private float delay = 3;
    private float time;

    private GameObject shark;
    public Vector3 sharkPos;
    public Quaternion directionDebug;

    public Vector3 goalPos;

    public FishFood food;
    public float foodlevel;
    public float maxFood;

    void Start() 
    {
        speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
        shark = GameObject.FindWithTag("Shark");
    }

    // Update is called once per frame
    void Update()
    {
        //
        //Debug.DrawRay(transform.position,transform.forward * 10,Color.red);
        Bounds b = new Bounds(flockManager.transform.position, flockManager.swimLimits);

        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;
        

        if (turningAwayFromPredator)
        {
            Debug.DrawRay(transform.position,transform.forward * 20,Color.red);
            sharkPos = shark.transform.position;
            direction = sharkPos + transform.position;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(-sharkPos),
                flockManager.rotationSpeed * Time.deltaTime * 1.5f);
            speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed) + 0.2f;
            time += Time.deltaTime;
            turning = true;
            if (time >= delay)
            {
                time = 0;
                turningAwayFromPredator = false;
                turning = false;
            }
        }
        
        
        
        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = flockManager.transform.position - transform.position;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward, out hit,0.005f))
        {
            Debug.DrawRay(transform.position,transform.forward,Color.green);
            if (hit.transform.CompareTag("Shark"))
            {
                turningAwayFromPredator = true;
                
            }
            if (Physics.Raycast(transform.position, this.transform.forward, out hit,0.005f))
            {
                turning = true;
                direction = Vector3.Reflect(transform.forward, hit.normal);
            }
        
            
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
           
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                flockManager.rotationSpeed  * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0,1000) < resetSpeedChance)
            {
                speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
            }

            if (Random.Range(0, 500) < applyRulesChance)
            {
                ApplyRules();
            }
        }

        directionDebug = Quaternion.Euler(direction);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);

        if (foodlevel <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void ApplyRules() 
    {
        List<GameObject> gos;
        gos = flockManager.allFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        if (food != null)
        {
            goalPos = food.foodPos;
            foodlevel = maxFood;
        }
        else
        {
            goalPos = flockManager.goalPos;
            if (Random.Range(0, 500) < applyRulesChance)
            {
                foodlevel -= Random.Range(0.1f, 1);
            }
        }

        foreach (GameObject go in gos) {

            if (go != this.gameObject && go != null) {

                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= flockManager.neighbourDistance) {

                    vcentre += go.transform.position;
                    groupSize++;

                    if (nDistance < 0.30f) {

                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    if (anotherFlock != null)
                    {
                        gSpeed = gSpeed + anotherFlock.speed;
                    }

                }
            }
        }

        if (groupSize > 0) 
        {
            
            if (!turningAwayFromPredator)
            {
                vcentre = vcentre / groupSize + (goalPos - this.transform.position);
            }
            else
            {
                vcentre = vcentre / groupSize;
            }
            speed = gSpeed / groupSize;

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero) {

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    flockManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
