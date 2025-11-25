using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Player");
        defaultState = AIState.Idle;
    }

    void Start()
    {
        enemyState = defaultState;
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
                //idle
                //pick random location on map as direction, walk there, look around
                Vector3 point;
            if (RandomPoint(transform.position, idleRange, out point)) 
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); 
                agent.SetDestination(point);
            }
            DetermineTerritory();
            break;
            case AIState.Patrol:
                //patrol
                //pick random area in a radius around the players last seen location, go there, look around, after set time return to idle
                patrolArea = agent.destination;
                // search radius around patrolArea
            DetermineTerritory();
            break;
            case AIState.Chase:
                agent.destination = player.transform.position;
                territoryState = TerritoryState.Danger;
                agent.destination = player.transform.position;
                enemyState = AIState.Patrol;
                
            
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
        if (other.tag == "Player")
        {
            UpdateEnemy();
            //Debug.Log("Enemy Collider Triggered");
        }
    }

    void UpdateEnemy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, layerMask))
        {
            rayDir = (player.GetComponent<Transform>().position - transform.position).normalized;
            Debug.DrawRay(transform.position, rayDir * hit.distance, Color.yellow);
            //Debug.DrawRay(transform.position, (player.GetComponent<Transform>().position - transform.position) * hit.distance, Color.yellow);
            //Debug.Log(hit.transform.name);
            
            if (hit.transform.name == "Player")
            {
                enemyState = AIState.Chase;
                Debug.Log("Chase activated");
            }
        }
        DetermineAction();
        
    }

    bool RandomPoint(Vector3 center, float idleRange, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * idleRange; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


    void UpdateTerritory()
    {
        DetermineTerritory();
    }

    void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            enemyState = AIState.Idle;
            DetermineAction();
            
        }
    }

}
