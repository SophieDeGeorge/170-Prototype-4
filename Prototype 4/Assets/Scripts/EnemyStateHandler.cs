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
    private Vector3 rayStart;
    [SerializeField] AIState defaultState;
    private Stack<Vector3> patrolPoints = new Stack<Vector3>();
    private int numPoints;
    [SerializeField] private float patrolRange;
    private bool playerIsSeen = false;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float idleSpeed;
    //[SerializeField] private GameObject creep_mesh;
    [SerializeField] private MonsterAnimScript mAnim;
    [SerializeField] private float rayHeightTest;


    void Awake()
    {
        mAnim = GetComponentInChildren<MonsterAnimScript>();
        //mAnim = creep_mesh.GetComponent<MonsterAnimScript>();
        layerMask = LayerMask.GetMask("Wall", "Player");
        defaultState = AIState.Idle;
    }

    void Start()
    {
        //mAnim = GetComponent<MonsterAnimScript>();
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
        //Debug.Log("trigger stay");
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player in collider");
            RaycastHit hit;
            
            rayStart = new Vector3(transform.position.x, transform.position.y + rayHeightTest, transform.position.z);
            rayDir = (player.GetComponent<Transform>().position - rayStart);
            if (Physics.Raycast(rayStart, rayDir.normalized, out hit, layerMask))
            {
                //rayDir = (player.GetComponent<Transform>().position - transform.position).normalized;
                //rayStart = ()
                Debug.DrawRay(rayStart, rayDir, UnityEngine.Color.yellow);

                if (hit.transform.name == "Player")
                {
                    playerIsSeen = true;
                    //Debug.Log("(127)PlayerIsSeen Changed to: " + playerIsSeen);
                    if (enemyState != AIState.Chase)
                    {
                        enemyState = AIState.Chase;
                        DetermineAction();
                    }
                    
                } else if (hit.transform.name != "Player")
                {
                    playerIsSeen = false;
                    //Debug.Log("(137) PlayerIsSeen Changed to: " + playerIsSeen);
                }
            }
        } else
        {
            //Debug.Log("Player not in collider");
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
            if (agent.remainingDistance == 0)
            {
                if (patrolPoints.Count == 0)
                {
                    Debug.Log("Out of patrol points, returning to idle");
                    enemyState = AIState.Idle;
                }
                DetermineAction();
            }
        }
        if (enemyState == AIState.Chase)
        {   
            RaycastHit hit;
            Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, layerMask);
            if (hit.transform.name == "Player")
            {
                playerIsSeen = true;
                //Debug.Log("(171) PlayerIsSeen Changed to: " + playerIsSeen);
            }
            agent.SetDestination(player.transform.position);
            if (!playerIsSeen)
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

    void UpdateAnimations()
    {
        if (enemyState == AIState.Idle)
        {
            if (agent.remainingDistance > 0)
            {
                mAnim.PlayAnimation(MonsterAnimScript.AnimState.Walk);
            } else
            {
                mAnim.PlayAnimation(MonsterAnimScript.AnimState.Idle);
            }
        } else if (enemyState == AIState.Patrol)
        {
            if (agent.remainingDistance > 0)
            {
                mAnim.PlayAnimation(MonsterAnimScript.AnimState.Run);
            } else
            {
                mAnim.PlayAnimation(MonsterAnimScript.AnimState.Idle);
            }
        } else if (enemyState == AIState.Chase)
        {
            mAnim.PlayAnimation(MonsterAnimScript.AnimState.Run);
        }
        //Debug.Log("enemyState: " + enemyState);

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
        UpdateAnimations();
        //Debug.Log("Player is Seen: " + playerIsSeen);
    }

}
