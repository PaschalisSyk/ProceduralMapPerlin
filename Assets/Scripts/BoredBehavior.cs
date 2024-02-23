using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoredBehavior : StateMachineBehaviour
{
    [SerializeField]
    private float timeUntilBored;

    [SerializeField]
    private int numberOfBoredAnimations;

    private bool isBored;
    private float idleTime;
    //private int _boredAnimation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(isBored == false && stateInfo.IsName("Idle"))
        {
            idleTime += Time.deltaTime;

            if(idleTime > timeUntilBored)
            {
                isBored = true;
                int _boredAnimation = Random.Range(0, numberOfBoredAnimations + 1);

                animator.SetInteger("BoredAnimIndex", _boredAnimation);
                animator.SetTrigger("isBored");
            }
        }
        else if(stateInfo.normalizedTime % 1 > 0.98)
        {
            ResetIdle(animator);
        }
    }

    void ResetIdle(Animator animator)
    {
        isBored = false;
        idleTime = 0;
    }
}
