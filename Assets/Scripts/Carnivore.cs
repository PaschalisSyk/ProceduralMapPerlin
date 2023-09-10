using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : AnimalController
{
    private bool isHunting = false;
    private Transform targetPrey;
    private float attackRange = 1.0f;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if(foodSource != null)
        {
            SetTargetPrey(foodSource);
        }
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();

        if (isHunting && targetPrey != null)
        {
            float distanceToPrey = Vector3.Distance(transform.position, targetPrey.position);

            if (distanceToPrey <= attackRange)
            {
                transform.LookAt(targetPrey);
                anim.SetBool("AttackPrey", true);
                // Carnivore is in attack range, trigger the bool on the prey
                Prey prey = targetPrey.GetComponent<Prey>();
                if (prey != null)
                {
                    prey.getsAttacked = true;
                }
                StartCoroutine(ResetAttackFlag());
            }
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
        isHunting = false;
    }

    public void SetTargetPrey(Transform preyTransform)
    {
        targetPrey = preyTransform;
        isHunting = true;
    }
}
