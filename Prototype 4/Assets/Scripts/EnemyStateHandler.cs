using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateHandler : MonoBehaviour
{


    [SerializeField] private GameObject player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float idleRange;
    private Vector3 patrolArea;
    LayerMask layerMask;
    private Vector3 rayDir;
    [SerializeField] AIState defaultState;
    private Stack<Vector3> patrolPoints = new Stack<Vector3>();
    private int numPoints;
    [SerializeField] private float patrolRange;
    private bool playerIsSeen;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float idleSpeed;


    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Player");
        defaultState = AIState.Idle;
    }

    void Start()
    {
        enemyState = defaultState;
        playerIsSeen = false;
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //Enum Declaration

    public enum AIState 
    {
        Idle,
        Patrol,
        Chase
    }

    public enum TerritoryState 
    {
        Neutral,
        Warning,
        Danger
    }
////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //State Object Types

    AIState enemyState;
    TerritoryState territoryState;

////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //State Switch Case Functions

    void DetermineAction()
    {
        switch (enemyState)
        {
            case AIState.Idle:
            //Debug.Log("idle");
                agent.speed = idleSpeed;
                Vector3 point;
                point = NewRandomPoint(transform.position, idleRange, out point);
                //Debug.DrawRay(point, Vector3.up, UnityEngine.Color.yellow, 100);
                //Debug.Log("Idle Destination: " + agent.destination);
                agent.SetDestination(point);
        
            break;
            case AIState.Patrol: //find the code that determines when patrol starts, setup patrolPoints[] there
                //Debug.Log("patrol");
                agent.SetDestination(patrolPoints.Pop());
            
            break;
            case AIState.Chase:
                //Debug.Log("chase");
                agent.speed = chaseSpeed;
                agent.SetDestination(player.transform.position);
            break;
        }
    }


    void DetermineTerritory()
    {
        switch (territoryState)
        {
            case TerritoryState.Neutral:
                //neutral
            break;
            case TerritoryState.Warning:
                //warning
            break;
            case TerritoryState.Danger:
                //danger
            break;
        }  
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, layerMask))
            {
                rayDir = (player.GetComponent<Transform>().position - transform.position).normalized;

                if ((hit.transform.name == "Player") && (enemyState != AIState.Chase))
                {
                    playerIsSeen = true;
                    enemyState = AIState.Chase;
                    DetermineAction();
                } else if (hit.transform.name != "Player")
                {
                    playerIsSeen = false;
                }
            }
        }
    }

    void UpdateEnemy()
    {
        if (enemyState == AIState.Idle)
        {
            if (agent.remainingDistance == 0)
            {
                DetermineAction();
            }
        }
        if (enemyState == AIState.Patrol)
        {

            //Debug.Log("Patrol Points Left" + patrolPoints.Count);
            //Debug.Log("Going to: " + agent.destination);
            if (agent.remainingDistance == 0)
            {
                if (patrolPoints.Count == 0)
                {
                    //Debug.Log("Out of patrol points, returning to idle");
                    enemyState = AIState.Idle;
                }
                DetermineAction();
            }
        }
        if (enemyState == AIState.Chase)
        {
            if (playerIsSeen)
            {
                agent.SetDestination(player.transform.position);
            } else
            {
                enemyState = AIState.Patrol;
                patrolPoints.Clear();
                numPoints = 2;
                //UnityEngine.Random.Range(2, 3);
                for (int i = 0; i < numPoints; i++)
                {
                    Vector3 point;
                    point = NewRandomPoint(agent.destination, patrolRange, out point);
                    //Debug.DrawRay(point, Vector3.up, UnityEngine.Color.yellow, 100);
                    patrolPoints.Push(point);
                }
                DetermineAction();
            }
        }
        
    }

    bool RandomPoint(Vector3 center, float idleRange, out Vector3 result)
    {

        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * idleRange; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            result = hit.position;
            return true;
        }
        
        result = Vector3.zero;
        return false;
    }

    Vector3 NewRandomPoint(Vector3 center, float idleRange, out Vector3 result)
    {

        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * idleRange; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            result = hit.position;
            return result;
        }
        return NewRandomPoint(center, idleRange, out result);
    }


    void UpdateTerritory()
    {
        DetermineTerritory();
    }

    void Update()
    {
        UpdateEnemy();
    }

}
