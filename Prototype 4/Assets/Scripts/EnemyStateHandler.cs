using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateHandler : MonoBehaviour
{


    [SerializeField] private GameObject player;
    [SerializeField] private NavMeshAgent agent;
    private Vector3 patrolArea;
    LayerMask layerMask;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Player");
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
                //chase
                //run straight to player as long as player is seen
                //while (Physics.Raycast(transform.position, (player.GetComponent<Transform>().position - transform.position, 100f, layerMask))) //raycast hits player
                //{
                    agent.destination = player.GetComponent<Transform>().position;
                    territoryState = TerritoryState.Danger;
                //}
                agent.destination = player.GetComponent<Transform>().position;
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
            Debug.Log("Enemy Collider Triggered");
        }
    }

    void UpdateEnemy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.GetComponent<Transform>().position - transform.position, out hit, layerMask))
        {
            Debug.DrawRay(transform.position, (player.GetComponent<Transform>().position - transform.position) * hit.distance, Color.yellow);
            //Debug.Log(hit.transform.name);
            
            if (hit.transform.name == "Player")
            {
                enemyState = AIState.Chase;
                Debug.Log("Chase activated");
            } else
            {
                Debug.Log("Hit Wall");  
            }
        }
        DetermineAction();
        
    }

    void UpdateTerritory()
    {
        DetermineTerritory();
    }

    void Update()
    {
        //DetermineAction;
        //DetermineTerritory;
    }

}
