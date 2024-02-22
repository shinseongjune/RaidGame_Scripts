using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public NavMeshAgent agent;
    ControlComponent control;

    float DASH_SPEED = 20f;
    float DASH_TIME = 0.2f;
    float restDashTime = 0;

    public float DASH_COOLDOWN = 2f;
    public float restDashCooldown = 0;
    Vector3 dashDirection;

    bool isDashing = false;
    bool isKnockBacked = false;

    Vector3 knockBackDirection;
    float restKnockBackTime = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent is not null)
        {
            agent.enabled = true;
        }
        control = GetComponent<ControlComponent>();
    }

    private void Update()
    {
        if (control.isDead || control.isEnd)
        {
            isKnockBacked = false;
            isDashing = false;
            CancelMove();
            return;
        }

        if (restDashCooldown > 0)
        {
            restDashCooldown = Mathf.Max(restDashCooldown - Time.deltaTime, 0);
        }

        if (isKnockBacked)
        {
            restKnockBackTime = Mathf.Max(restKnockBackTime - Time.deltaTime, 0);

            agent.Move(knockBackDirection * Time.deltaTime);

            if (restKnockBackTime <= 0)
            {
                isKnockBacked = false;
                EnableMovement();
                control.EndMovement();
            }
        }
        else if (isDashing)
        {
            restDashTime = Mathf.Max(restDashTime - Time.deltaTime, 0);

            agent.Move(dashDirection * DASH_SPEED * Time.deltaTime);

            if (restDashTime <= 0)
            {
                isDashing = false;
                EnableMovement();
                control.EndMovement();
            }
        }
        else if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance)
        {
            CancelMove();
        }
    }

    public void MoveTo(Vector3 destination)
    {
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(destination, out navMeshHit, 100f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(navMeshHit.position);
        }
    }

    public void CancelMove()
    {
        if (agent.enabled)
        {
            agent.SetDestination(transform.position);
            agent.isStopped = true;
        }
        control.EndMovement();
    }

    public void EnableMovement()
    {
        CancelMove();
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
    }

    public void GetKnockBack(Vector3 direction)
    {
        CancelMove();
        restKnockBackTime = KnockBack.KnockBack_Time;
        knockBackDirection = direction;
        isKnockBacked = true;
    }

    public void Dash(Vector3 point, Vector3 direction)
    {
        if (restDashCooldown <= 0)
        {
            CancelMove();
            control.Look(point);
            dashDirection = direction;
            restDashTime = DASH_TIME;
            isDashing = true;

            restDashCooldown = DASH_COOLDOWN;
        }
    }

    public void SetStoppingDistance(float dist)
    {
        if (agent.enabled)
        {
            agent.stoppingDistance = dist;
        }
    }

    public bool IsReached()
    {
        return agent.remainingDistance <= 0.1f;
    }
}
