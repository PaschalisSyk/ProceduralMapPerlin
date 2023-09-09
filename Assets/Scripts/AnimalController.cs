using UnityEngine;
using UnityEngine.AI;

public class AnimalController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    private Vector3 wanderTarget;
    private float zoneRadius = 20f;
    private float wanderTimer = 5f;
    [HideInInspector]
    public Transform foodSource;
    [HideInInspector]
    public Animator anim;

    [SerializeField] float speed;
    [SerializeField] string food;


    [SerializeField] private float hungerLevel = 100f; // Starting hunger level
    private float hungerDecreaseRate = 2f; // Rate at which hunger decreases over time
    private float eatTimer = 0f;
    private float eatingDuration = 2.5f;
    [SerializeField] private float timeSpentAtCurrentFoodSource = 0f;
    private float maxTimeAtFoodSource = 10f;

    [SerializeField] private bool isHungry = false;
    private bool isEating = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        SetRandomDestination();
    }

    protected virtual void Update()
    {
        speed = agent.velocity.magnitude;
        HandleAnimation();
        HungerController(food);

        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Wander();
        }
        //if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        //{
        //    transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        //}
    }

    private void Wander()
    {
        // If not hungry and reached the current destination, set a new random destination for wandering.
        switch (wanderTimer)
        {
            case <= 0f:
                SetRandomDestination(); // Set a new random destination
                wanderTimer = 5f; // Reset the timer to its initial duration
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

        if (hungerLevel <= 0 && !isHungry)
        {
            // The animal is hungry, trigger food-seeking behavior
            isHungry = true;
            FindFoodSource(food);
        }

        if(isHungry)
        {
            timeSpentAtCurrentFoodSource += Time.deltaTime;

            // Check if the time limit has been exceeded, and if so, find a new food source
            if (timeSpentAtCurrentFoodSource >= maxTimeAtFoodSource && !isEating)
            {
                FindFoodSource(food); // Replace "Food" with the appropriate food tag
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
            }
        }
    }

    private void FindFoodSource(string food)
    {
        GameObject[] foodSources = GameObject.FindGameObjectsWithTag(food); // Change "Food" to your food object tag

        // Find the nearest food source within the search radius
        Transform nearestSource = null;
        float nearestDistance = zoneRadius; // Initialize to the search radius

        foreach (GameObject foodObject in foodSources)
        {
            Transform sourceTransform = foodObject.transform;
            float distance = Vector3.Distance(transform.position, sourceTransform.position);

            // Check if this source is within the search radius and closer than the previous closest
            if (distance < zoneRadius && distance < nearestDistance)
            {
                nearestSource = sourceTransform;
                nearestDistance = distance;
            }
        }

        // Set the nearest food source as the target
        foodSource = nearestSource;
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
                hungerLevel = 100f;

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
}
