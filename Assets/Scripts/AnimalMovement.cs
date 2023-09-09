using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalMovement : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    public float stoppingDistance = 2.0f;
    Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the agent is not too close to the player before setting the destination.
        if (distanceToPlayer > stoppingDistance)
        {
            anim.SetBool("HasArrived", false);
            agent.destination = player.position;
        }
        else
        {
            // If the agent is too close to the player, stop moving.
            agent.destination = transform.position;
            anim.SetBool("HasArrived", true);
        }
    }
}
