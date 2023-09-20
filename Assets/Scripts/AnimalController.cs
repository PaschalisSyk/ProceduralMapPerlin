using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class AnimalController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    private Vector3 wanderTarget;
    [SerializeField] private float zoneRadius = 20f;
    private float wanderTimer = 5f;
    //[HideInInspector]
    public Transform foodSource;
    [HideInInspector]
    public Animator anim;

    [SerializeField] float speed;
    [SerializeField] string food;
    [SerializeField] float baseHunger = 100f;

    [SerializeField] private float hungerLevel; // Starting hunger level
    private float hungerDecreaseRate = 2f; // Rate at which hunger decreases over time
    //[SerializeField] private float eatTimer = 0f;
    //private float eatingDuration = 2.5f;
    [SerializeField] private float timeSpentAtCurrentFoodSource = 0f;
    private float maxTimeAtFoodSource = 15f;
    [SerializeField] private float starvationThreshold = -400f;

    [SerializeField] private float staminaConsumptionRate;
    [SerializeField] private float staminaRegenerationRate = 5f;
    [SerializeField] private float maxStamina = 100f;
    public float currentStamina;

    [SerializeField] private bool isHungry = false;
    [HideInInspector]
    public bool isEating = false;

    public bool isFleeing = false;
    public float fleeDistance = 4f;
    private Transform player;

    protected virtual void Start()
    {
        wanderTimer = Random.Range(8f, 15f);
        currentStamina = maxStamina;
        agent = GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        hungerLevel = Random.Range(10, 150);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDestination();
    }

    protected virtual void Update()
    {
        HandleAnimalBehavior();
    }

    private void HandleAnimalBehavior()
    {
        speed = agent.velocity.magnitude;
        ConsumeStamina();
        RegenerateStamina();
        HandleAnimation();
        HungerController(food);

        if (!isEating)
        {
            Flee(player);
        }

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            if(!isEating)
            {
                Wander();
            }
        }
        if (currentStamina == 0)
        {
            //Wander();
            //foodSource = null;
            agent.speed = agent.speed/2;
            Invoke("ResetStamina", 3f);
        }
    }

    private void Wander()
    {
        // If not hungry and reached the current destination, set a new random destination for wandering.
        switch (wanderTimer)
        {
            case <= 0f:
                SetRandomDestination(); // Set a new random destination
                wanderTimer = Random.Range(3f, 8f); // Reset the timer to its initial duration
                break;
            default:
                wanderTimer -= Time.deltaTime; // Decrease the timer
                break;
        }
    }

    private void SetRandomDestination()
    {
        wanderTarget = RandomNavSphere(transform.position, zoneRadius, -1);
        agent.SetDestination(wanderTarget);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    private void HungerController(string food)
    {
        // Decrease hunger over time
        hungerLevel -= hungerDecreaseRate * Time.deltaTime;

        if (hungerLevel <= starvationThreshold)
        {
            // The animal is starving, implement starving behavior here
            StarveToDeath();
        }

        if (!isFleeing)
        {
            if (hungerLevel <= 0 && !isHungry)
            {
                // The animal is hungry, trigger food-seeking behavior
                isHungry = true;
                FindFoodSource(food);
            }

            if (isHungry)
            {
                timeSpentAtCurrentFoodSource += Time.deltaTime;

                // Check if the time limit has been exceeded, and if so, find a new food source
                if (timeSpentAtCurrentFoodSource >= maxTimeAtFoodSource && !isEating)
                {
                    FindFoodSource(food);
                    timeSpentAtCurrentFoodSource = 0;
                }
            }

            if (isHungry && foodSource != null)
            {
                // Move towards the food source
                agent.SetDestination(foodSource.position);

                // Check if the animal has reached the food source
                if (Vector3.Distance(transform.position, foodSource.position) < 1f)
                {
                    // Interact with the food source (e.g., consume it)
                    isEating = true;
                    EatFood();
                    timeSpentAtCurrentFoodSource = 0;
                }
            }
        }
    }

    private void StarveToDeath()
    {
        anim.SetBool("IsStarving", true);
        agent.velocity = Vector3.zero;
        Destroy(gameObject, 5f);
    }

    private void FindFoodSource(string food)
    {
        GameObject[] foodSources = GameObject.FindGameObjectsWithTag(food); // Change "Food" to your food object tag

        // Create a list to store all nearby food sources
        List<Transform> nearbySources = new List<Transform>();

        // Find all food sources within the search radius
        foreach (GameObject foodObject in foodSources)
        {
            Transform sourceTransform = foodObject.transform;
            float distance = Vector3.Distance(transform.position, sourceTransform.position);

            // Check if this source is within the search radius
            if (distance < zoneRadius)
            {
                nearbySources.Add(sourceTransform);
            }
        }

        // Check if there are any nearby food sources
        if (nearbySources.Count > 0)
        {
            // Randomly select one of the nearby food sources
            int randomIndex = Random.Range(0, nearbySources.Count);
            foodSource = nearbySources[randomIndex];
        }
        else
        {
            // No nearby food sources found, reset the current food source
            foodSource = null;
        }
    }

    private void EatFood()
    {
        StartCoroutine(ResetEating());
        //if (isEating)
        //{
        //    // Increment the eat timer
        //    eatTimer += Time.deltaTime;

        //    // Check if the eat timer has reached the desired eating duration
        //    if (eatTimer >= eatingDuration)
        //    {
        //        // Finish eating
        //        isEating = false;
        //        isHungry = false;

        //        // Reset the hunger level (you can set it to your desired starting value)
        //        hungerLevel = baseHunger;

        //        // Reset the eat timer
        //        eatTimer = 0f;
        //    }
        //}
    }

    protected virtual void HandleAnimation()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            anim.SetBool("HasArrived", false);
        }
        if (agent.velocity.magnitude < 0.1f)
        {
            anim.SetBool("HasArrived", true);
        }
        if (isEating)
        {
            anim.SetBool("IsEating", true);
        }
        if (!isEating)
        {
            anim.SetBool("IsEating", false);
        }
    }

    public void Flee(Transform predatorTransform)
    {
        float distance = Vector3.Distance(transform.position, predatorTransform.position);

        //if(distance < fleeDistance)
        //{
        //    Vector3 dirToPlayer = transform.position - predatorTransform.position;

        //    Vector3 newPos = transform.position + dirToPlayer;

        //    agent.SetDestination(newPos);
        //}
        if (distance < fleeDistance)
        {
            // calculate the flee direction away from the predator
            Vector3 fleedirection = transform.position - predatorTransform.position;

            // add a random offset to the flee direction to make it less predictable
            float fleeangle = Random.Range(-90f, 90f); // adjust the angle range as needed
            Quaternion randomrotation = Quaternion.Euler(0, fleeangle, 0);
            Vector3 randomoffset = randomrotation * fleedirection;

            // calculate the new destination point with the random offset
            Vector3 newdestination = transform.position + randomoffset.normalized * fleeDistance;
            //vector3 newdestination = transform.position + fleedirection * fleedistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(newdestination, out hit, fleeDistance, NavMesh.AllAreas))
            {
                // set the new destination for the animal
                agent.SetDestination(newdestination);
                Debug.DrawLine(transform.position, newdestination, Color.red);

            }
            else
            {
                SetRandomDest();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered the trigger zone, initiate flee behavior
            isFleeing = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player exited the trigger zone, stop fleeing behavior
            isFleeing = false;
        }
    }

    private void ConsumeStamina()
    {
        // Calculate the current speed of the agent
        float currentSpeed = agent.velocity.magnitude;

        // Calculate the stamina consumption based on speed
        float staminaConsumption = currentSpeed * staminaConsumptionRate * Time.deltaTime;

        // Decrease stamina
        currentStamina -= staminaConsumption;
        if (currentStamina < 0)
        {
            currentStamina = 0;
        }

    }
    private void RegenerateStamina()
    {
        // Check if the agent's velocity is zero (not moving)
        if (agent.velocity.magnitude < 0.01f)
        {
            // Add logic to regenerate stamina when not moving
            currentStamina += staminaRegenerationRate * Time.deltaTime;

            // Clamp stamina to the maximum value
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    private void ResetStamina()
    {
        currentStamina = maxStamina;
        agent.speed = agent.speed * 2;
    }

    private IEnumerator ResetEating()
    {
        yield return new WaitForSeconds(2.5f);
        // Finish eating
        isEating = false;
        isHungry = false;

        // Reset the hunger level (you can set it to your desired starting value)
        hungerLevel = baseHunger;
    }

    private void SetRandomDest()
    {
        // Get a random point on the NavMesh surface
        Vector3 randomPoint = RandomNavMeshPoint();

        // Set the agent's destination to the random point
        agent.SetDestination(randomPoint);
    }

    private Vector3 RandomNavMeshPoint()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick a random triangle from the NavMesh triangles
        int randomTriangleIndex = Random.Range(0, navMeshData.indices.Length / 3);
        int index1 = navMeshData.indices[randomTriangleIndex * 3];
        int index2 = navMeshData.indices[randomTriangleIndex * 3 + 1];
        int index3 = navMeshData.indices[randomTriangleIndex * 3 + 2];

        // Calculate a random point within the selected triangle
        Vector3 randomPoint = Vector3.Lerp(navMeshData.vertices[index1], navMeshData.vertices[index2], Random.value);
        randomPoint = Vector3.Lerp(randomPoint, navMeshData.vertices[index3], Random.value);

        return randomPoint;
    }
}