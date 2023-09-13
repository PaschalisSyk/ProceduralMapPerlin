using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AnimalController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    private Vector3 wanderTarget;
    private float zoneRadius = 20f;
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
    [SerializeField] private float eatTimer = 0f;
    private float eatingDuration = 2.5f;
    [SerializeField] private float timeSpentAtCurrentFoodSource = 0f;
    private float maxTimeAtFoodSource = 10f;
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

        if (!agent.hasPath || agent.remainingDistance < 1f && !isEating && !isFleeing)
        {
            Wander();
        }
        if (currentStamina == 0)
        {
            Wander();
            foodSource = null;
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
                wanderTimer = Random.Range(3f,8f); // Reset the timer to its initial duration
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

        if(!isFleeing)
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
        if (isEating)
        {
            // Increment the eat timer
            eatTimer += Time.deltaTime;

            // Check if the eat timer has reached the desired eating duration
            if (eatTimer >= eatingDuration)
            {
                // Finish eating
                isEating = false;
                isHungry = false;

                // Reset the hunger level (you can set it to your desired starting value)
                hungerLevel = baseHunger;

                // Reset the eat timer
                eatTimer = 0f;
            }
        }
    }

    protected virtual void HandleAnimation()
    {
        if(agent.velocity.magnitude > 0.1f)
        {
            anim.SetBool("HasArrived", false);
        }
        if(agent.velocity.magnitude < 0.1f)
        {
            anim.SetBool("HasArrived", true);
        }
        if(isEating)
        {
            anim.SetBool("IsEating", true);
        }
        if(!isEating)
        {
            anim.SetBool("IsEating", false);
        }
    }

    public void Flee(Transform predatorTransform)
    {
        if (isFleeing && agent.remainingDistance < 1f)
        {
            // Calculate the flee direction away from the predator
            Vector3 fleeDirection = transform.position - predatorTransform.position;

            // Add a random offset to the flee direction to make it less predictable
            float fleeAngle = Random.Range(-90f, 90f); // Adjust the angle range as needed
            Quaternion randomRotation = Quaternion.Euler(0, fleeAngle, 0);
            Vector3 randomOffset = randomRotation * fleeDirection;

            // Calculate the new destination point with the random offset
            Vector3 newDestination = transform.position + randomOffset.normalized * fleeDistance;
            //Vector3 newDestination = transform.position + fleeDirection * fleeDistance;
            Debug.DrawRay(transform.position, newDestination, Color.red);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(newDestination, out hit, fleeDistance, NavMesh.AllAreas))
            {
                // Set the new destination for the animal
                agent.SetDestination(hit.position);
            }
            else
            {
                agent.SetDestination(newDestination);
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
        if(currentStamina < 0)
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
    }
}
