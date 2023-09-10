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

    private void FixedUpdate()
    {
        Test();
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();
        if(getsAttacked)
        {
            this.anim.SetBool("GotHit", true);
        }
    }
    void Test()
    {
        float number_of_rays = 8;
        float totalAngle = 360;

        float delta = totalAngle / number_of_rays;
        Vector3 pos = transform.position;
        const float magnitude = 5;

        for (int i = 0; i < number_of_rays; i++)
        {
            var dir = Quaternion.Euler(0, i * delta, 0) * transform.right;

            RaycastHit hitInfo;
            if (Physics.Raycast(pos, dir, out hitInfo, 5f))
            {
                // Check if the ray hit a collider with the "Predator" tag
                if (hitInfo.collider.CompareTag("Snake"))
                {
                    if (Random.value < 0.5f)
                    {
                        isFleeing = true;
                        Flee(hitInfo.collider.transform);
                        Debug.Log("Predator detected!");
                        StartCoroutine(ResetFleeing());
                    }
                }
            }

            Debug.DrawRay(pos, dir * magnitude, Color.green);
        }
    }

    IEnumerator ResetFleeing()
    {
        yield return new WaitForSeconds(2f);
        isFleeing = false;
    }
}
