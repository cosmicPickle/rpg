using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent2D))]
[RequireComponent(typeof(Character))]
public class BotNavAgent : MonoBehaviour
{


    Character character;
    NavMeshAgent2D agent;

    Vector3 pausedDestination;
    bool paused;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent2D>();
        character = GetComponent<Character>();
    }

    public Vector3 GetVelocity()
    {
        return agent.velocity;
    }

    public void Pause()
    {
        pausedDestination = agent.destination;
        paused = true;

        Stop();
    }

    public void Resume()
    {
        paused = false;
        agent.SetDestination(pausedDestination);
    }

    public void SetDestination(Vector3 destination, float stoppingDistance = 0f)
    {
        agent.stoppingDistance = stoppingDistance;

        agent.speed = character.defenceStats.speed.GetValue();

        if (paused)
        {
            pausedDestination = destination;
        } else
        {
            agent.SetDestination(destination);
        }
    }

    public void Stop()
    {
        if (agent == null)
            return;

        agent.SetDestination(transform.position);
    }
}
