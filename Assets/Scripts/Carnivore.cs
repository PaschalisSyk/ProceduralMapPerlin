using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : AnimalController
{
    //private bool isHunting = false;
    private Transform targetPrey;
    private float attackRange = 1.0f;
    protected override void Start()
    {
        base.Start();
        //InvokeRepeating("CarnivoreMovement", 2f, 2f);
    }

    protected override void Update()
    {
        base.Update();
        if (foodSource != null)
        {
            CarnivoreMovement();
        }
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("AttackPrey", false);
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // After 2 seconds, set the isAttacking flag back to false
        anim.SetBool("IsEating", false);
        isEating = false;
        //isHunting = false;
    }

    public void SetTargetPrey(Transform preyTransform)
    {
        targetPrey = preyTransform;
        //isHunting = true;
    }
    private void StopChasing()
    {
        //isHunting = false;
        targetPrey = null;
    }

    private void CarnivoreMovement()
    {
        //// Check for available prey
        //if (foodSource != null && !isHunting)
        //{
        //    // Start chasing the prey
        //    SetTargetPrey(foodSource);
        //}

        if (foodSource != null)
        {
            float distanceToPrey = Vector3.Distance(transform.position, foodSource.position);
            Prey prey = foodSource.GetComponent<Prey>();

            if (distanceToPrey < attackRange && !prey.getsAttacked)
            {
                transform.LookAt(foodSource);
                anim.SetBool("AttackPrey", true);

                //// Check if the carnivore should attack based on the random decision
                //if (SucceedAttack())
                //{
                    
                //}
                if (prey != null)
                {
                    prey.getsAttacked = true;
                }
                StartCoroutine(ResetAttackFlag());
            }
        }
    }

    private bool SucceedAttack()
    {
        // Adjust the probability threshold as needed
        float attackProbability = 0.35f;

        // Check if the random value is below the attack probability
        return Random.value < attackProbability;
    }

    //protected override void EatFood()
    //{
    //    if(SucceedAttack())
    //    {
    //        StartCoroutine(ResetEating());
    //    }
    //}
}
