using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : AnimalController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();
        if(foodSource != null)
        {
            if (Vector3.Distance(transform.position, foodSource.position) < 1f)
            {
                anim.SetBool("AttackPrey", true);
                foodSource.gameObject.GetComponent<Prey>().getsAttacked = true;
                StartCoroutine(ResetAttackFlag());
            }
        }
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("AttackPrey", false);
        // Wait for 2 seconds
        yield return new WaitForSeconds(1.5f);

        // After 2 seconds, set the isAttacking flag back to false
        anim.SetBool("IsEating", false);
        isEating = false;
    }
}
