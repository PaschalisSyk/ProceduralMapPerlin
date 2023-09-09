using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : AnimalController
{
    public bool getsAttacked = false;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (getsAttacked)
        {
            agent.velocity = Vector3.zero;
            Destroy(gameObject, 3f);
        }
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();
        if(getsAttacked)
        {
            this.anim.SetBool("GotHit", true);
        }
    }
}
