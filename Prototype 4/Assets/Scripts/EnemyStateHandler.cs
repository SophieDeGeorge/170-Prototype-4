using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateHandler : MonoBehaviour
{


    [SerializeField] private GameObject player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float idleRange;
    private UnityEngine.Vector3 patrolArea;
    LayerMask layerMask;
    private UnityEngine.Vector3 rayDir;
    private UnityEngine.Vector3 rayStart;
    [SerializeField] AIState defaultState;
    private Stack<UnityEngine.Vector3> patrolPoints = new Stack<UnityEngine.Vector3>();
    private int numPoints;
    [SerializeField] private float patrolRange;
    private bool playerIsSeen = false;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float idleSpeed;
    [SerializeField] private MonsterAnimScript mAnim;
    [SerializeField] private float rayHeightTest;
    [SerializeField] private GameObject rayStartObject;
    private UnityEngine.Vector3 rayStartPoint;
    private UnityEngine.Vector3 rayStartTF;
    private TerritoryEffects tEffects;


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
        tEffects = player.GetComponentInChildren<TerritoryEffects>();
        enemyState = defaultState;
        rayStartPoint = rayStartObject.GetComponent<Transform>().position;
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
                UnityEngine.Vector3 point;
                point = NewRandomPoint(transform.position, idleRange, out point);
                //Debug.DrawRay(point, UnityEngine.Vector3.up, UnityEngine.Color.yellow, 100);
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
        if (enemyState == AIState.Chase)
        {
            territoryState = TerritoryState.Danger;
        } else if (enemyState == AIState.Patrol)
        {
            territoryState = TerritoryState.Warning;
        } else
        {
            territoryState = TerritoryState.Neutral;
        }
        switch (territoryState)
        {
            case TerritoryState.Neutral:
                tEffects.NeutralTerritory();
            break;
            case TerritoryState.Warning:
                tEffects.WarningTerritory();
            break;
            case TerritoryState.Danger:
                tEffects.DangerTerritory();
            break;
        }  
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (RaycastHitPlayer())
            {
                playerIsSeen = true;
                if (enemyState != AIState.Chase)
                {
                    enemyState = AIState.Chase;
                    DetermineAction();
                }
            } else
            {
                playerIsSeen = false;
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
            
            if (RaycastHitPlayer())
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
                    UnityEngine.Vector3 point;
                    point = NewRandomPoint(agent.destination, patrolRange, out point);
                    //Debug.DrawRay(point, UnityEngine.Vector3.up, UnityEngine.Color.yellow, 100);
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

    private bool RaycastHitPlayer()
    {
            RaycastHit hit;
            rayStartPoint = rayStartObject.GetComponent<Transform>().position;
            rayStartTF = rayStartObject.GetComponent<Transform>().position;
            rayStartPoint = new UnityEngine.Vector3(rayStartTF.x, 3, rayStartTF.z);
            rayDir = player.GetComponent<Transform>().position - rayStartPoint;

            Physics.Raycast(rayStartPoint, rayDir.normalized, out hit, layerMask);
            Debug.DrawRay(rayStartPoint, rayDir, UnityEngine.Color.yellow);
            
            if (hit.transform.name == "Player")
            {
                return true;
            } else
        {
            return false;
        }
            
    }


    UnityEngine.Vector3 NewRandomPoint(UnityEngine.Vector3 center, float idleRange, out UnityEngine.Vector3 result)
    {

        UnityEngine.Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * idleRange; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            result = hit.position;
            return result;
        }
        return NewRandomPoint(center, idleRange, out result);
    }

    void Update()
    {
        UpdateEnemy();
        UpdateAnimations();
        DetermineTerritory();
    }

}
