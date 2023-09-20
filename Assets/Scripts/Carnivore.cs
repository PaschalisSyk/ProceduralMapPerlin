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
        CarnivoreMovement();
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

            if (distanceToPrey < attackRange)
            {
                transform.LookAt(foodSource);
                anim.SetBool("AttackPrey", true);

                // Carnivore is in attack range, trigger the bool on the prey
                Prey prey = foodSource.GetComponent<Prey>();
                if (prey != null)
                {
                    prey.getsAttacked = true;
                }
                StartCoroutine(ResetAttackFlag());
            }
        }
    }
}
