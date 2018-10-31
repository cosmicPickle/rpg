using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(BotNavAgent))]
[RequireComponent(typeof(AttackAgent))]
[RequireComponent(typeof(FaceController))]
public class Bot : MonoBehaviour {

    [Header("General Settings")]
    public float friendDetectionRadius = 5;
    public LayerMask friendlyMask;

    public float enemyDetectionRadius = 4;
    public LayerMask enemyMask;
    public bool enableFieldOfView = true;
    public float fieldOfViewAngle = 80;
    public float enemyHearRadius = 2;

    [Header("Idle Settings")]
    public bool idleOnlyOnTargets;
    public float idleMinTime = 1;
    public float idleMaxTime = 2;

    [HideInInspector]
    public float idleTimeLeft = 0;

    [Header("Patrol Settings")]
    public PatrolType patrolType;
    public float patrolMinRadius = 5;
    public float patrolMaxRadius = 8;
    public float patrolMinTime = 6;
    public float patrolMaxTime = 12;

    public Vector3[] patrolTargets;
    public int maxPatrolTargets = 3;

    [HideInInspector]
    public const float minPatrolStoppingDistance = 0.01f;

    [HideInInspector]
    public float patrolTimeLeft = 0;
    [HideInInspector]
    public float patrolRadius;
    int currentPatrolTarget = 0;
    

    [Header("Follow Settings")]
    public bool allowFollow = false;
    public Collider2D permanentFollowTarget;
    public float maxFollowTargetDistance = 5f;
    public float friendlyStoppingDistance = 0.5f;
    public float followTargetRefreshTime = .5f;

    [Header("Attack Settings")]
    public float attackTargetRefreshTime = .5f;
    public Disposition disposition;

    [Header("Debug Settings")]
    public bool showTreeDebugInfo = false;
    public bool showEnemyDetectionInfo = true;
    public bool showEnemyLink = true;
    public bool showFriendDetectionRadius = true;
    public bool showFrindlyLink = false;
    public bool showPatrolTargets = true;

    //Status
    protected Collider2D target;
    protected Collider2D followTarget;

    [HideInInspector]
    public bool targetInRange;
    [HideInInspector]
    public bool targetIsFriendly;

    float targetRefershTime;

    BoxCollider2D ctrlCollider;
    DecisionTree<Bot, State> stateDecision;
    Vector3 initialPosition;

    [HideInInspector]
    public IWalker walker;
    [HideInInspector]
    public AttackAgent attackAgent;
    [HideInInspector]
    public FaceController faceController;

    protected State previousState;
    protected State currentState;

    public void Awake()
    {
        ctrlCollider = GetComponent<BoxCollider2D>();
        walker = GetComponent<BotNavAgent>();
        attackAgent = GetComponent<AttackAgent>();
        faceController = GetComponent<FaceController>();

        attackAgent.onAttackStart += walker.Pause;
        attackAgent.onAttackComplete += walker.Resume;
        attackAgent.Init(enemyMask, walker.GetObstacleMask());

        currentState = previousState = State.Idle;
        initialPosition = transform.position;

        InitState();
        GenerateDecisionTree();
    }

    void Update()
    {
        DetectTarget();
        currentState = stateDecision.Evaluate(this, showTreeDebugInfo);

        if(currentState != previousState)
        {
            InitState();
        } 

        ExecuteState();
        previousState = currentState;

    }

    void InitState()
    {
        walker.Stop();
        switch (currentState) {
            case State.Idle:
                idleTimeLeft = Random.Range(idleMinTime, idleMaxTime);break;
            case State.Patrol:
                patrolTimeLeft = Random.Range(patrolMinTime, patrolMaxTime);
                patrolRadius = Random.Range(patrolMinRadius, patrolMaxRadius);

                if (patrolType == PatrolType.Random || patrolTargets == null || patrolTargets.Length == 0)
                {
                    patrolTargets = new Vector3[maxPatrolTargets];
                    for(int i = 0; i < maxPatrolTargets; i++)
                    {
                        Vector2 randomPos = (Vector2)initialPosition + Random.insideUnitCircle * patrolRadius;

                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(NavMeshUtils2D.ProjectTo3D(randomPos), out hit, 2.0f, NavMesh.AllAreas))
                        {
                            patrolTargets[i] = NavMeshUtils2D.ProjectTo2D(hit.position);
                        }
                    }
                }

                walker.SetDestination(patrolTargets[currentPatrolTarget]);
                break;
            case State.Attack:
                targetRefershTime = attackTargetRefreshTime;
                walker.SetDestination(target.transform.position, attackAgent.range * .8f);
                faceController.UpdateFaceDirection(target.transform);
                break;
            case State.Follow:
                targetRefershTime = followTargetRefreshTime;
                float stoppingDistance = (followTarget.bounds.size.x > followTarget.bounds.size.y
                    ? followTarget.bounds.size.x
                    : followTarget.bounds.size.y
                ) + friendlyStoppingDistance;
                targetRefershTime = followTargetRefreshTime;
                walker.SetDestination(followTarget.transform.position);
                faceController.UpdateFaceDirection(followTarget.transform);
                break;
            default: break;
        }

    }

    void ExecuteState()
    {
        switch (currentState)
        {
            case State.Idle:
                idleTimeLeft -= Time.deltaTime; break;
            case State.Patrol:
                patrolTimeLeft -= Time.deltaTime;
                if ((patrolTargets[currentPatrolTarget] - transform.position).magnitude <= minPatrolStoppingDistance)
                {
                    currentPatrolTarget++;
                    if (currentPatrolTarget >= patrolTargets.Length)
                        currentPatrolTarget = 0;

                    Vector3 target = patrolTargets[currentPatrolTarget];
                    walker.SetDestination(target);
                }
                faceController.UpdateFaceDirection(walker.GetVelocity() * Time.deltaTime);
                break;
            case State.Attack:
                targetRefershTime -= Time.deltaTime;
                if(targetRefershTime <= 0)
                {
                    targetRefershTime = attackTargetRefreshTime;
                    walker.SetDestination(target.transform.position, attackAgent.range * .6f);
                    faceController.UpdateFaceDirection(target.transform);

                    if ((target.transform.position - transform.position).magnitude <= attackAgent.range)
                    {
                        attackAgent.Attack(target.transform);
                    }
                }
                break;
            case State.Follow:
                targetRefershTime -= Time.deltaTime;
                if (targetRefershTime <= 0)
                {
                    targetRefershTime = followTargetRefreshTime;
                    float stoppingDistance = (followTarget.bounds.size.x > followTarget.bounds.size.y 
                        ? followTarget.bounds.size.x 
                        : followTarget.bounds.size.y
                    ) + friendlyStoppingDistance;

                    walker.SetDestination(followTarget.transform.position, stoppingDistance);
                    faceController.UpdateFaceDirection(followTarget.transform);
                }
                break;
            default: break;
        }
    }

    void DetectTarget()
    {

        if (DetectEnemy())
            return;

        if (DetectFriend())
            return;

        UnsetTarget();
    }

    bool DetectEnemy()
    {
        //If the disposition is passive or defensive we will handle enemy detection 
        if (disposition == Disposition.Passive)
        {
            return false;
        }

        if(disposition == Disposition.Defensive)
        {
            Character character;
            if (followTarget != null && (character = followTarget.GetComponent<Character>()) != null && character.lastAttacker != null)
            {
                SetTarget(character.lastAttacker.GetComponent<Collider2D>(), false);
                return true;
            } else
            {
                character = GetComponent<Character>();
                if(character != null && character.lastAttacker)
                {
                    SetTarget(character.lastAttacker.GetComponent<Collider2D>(), false);
                    return true;
                }
                return false;
            }
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, enemyDetectionRadius, enemyMask);

        //Check if there is an enemy within range
        if (hit && hit != ctrlCollider)
        {
            //We only need to perform the following checks if the currenState isn't Attack

            if (currentState != State.Attack)
            {
                Vector2 direction = hit.transform.position - transform.position;
                float angle = Vector2.Angle(direction, faceController.GetFaceDirection());
                //If FOV is enabled check if enemy is within FOV or within hear radius
                if (!enableFieldOfView || angle < fieldOfViewAngle * 0.5 || direction.magnitude <= enemyHearRadius)
                {
                    //Check if target is behind obstacle
                    RaycastHit2D obstacle = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, walker.GetObstacleMask());

                    if (!obstacle)
                    {
                        SetTarget(hit, false);
                        return true;
                    }
                }
            } else
            {
                SetTarget(hit, false);
                return true;
            }
        }

        return false;
    }

    bool DetectFriend()
    {
        if(permanentFollowTarget != null)
        {
            SetTarget(permanentFollowTarget, true);
            
            return true;
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, friendDetectionRadius, friendlyMask);

        if (hit && hit != ctrlCollider)
        {
            SetTarget(hit, true);
            return true;
        }

        return false;
    }

    void SetTarget(Collider2D hit, bool friendly)
    {

        if (friendly)
        {
            followTarget = hit;
            target = null;
        }
        else
        {
            target = hit;
            if (permanentFollowTarget == null)
            {
                followTarget = null;
            }
        }

        targetInRange = true;
        targetIsFriendly = friendly;

        
    }

    void UnsetTarget()
    {
        target = null;
        if (permanentFollowTarget == null)
        {
            followTarget = null;
        }
        targetInRange = false;
        targetIsFriendly = false;
    }

    void GenerateDecisionTree()
    {
        stateDecision = new DecisionTree<Bot, State>
        {
            root = new DecisionTree<Bot, State>.Node
            {
                Condition = bot => bot.targetInRange,
                positive = new DecisionTree<Bot, State>.Node
                {
                    Condition = bot => bot.targetIsFriendly,
                    positive = new DecisionTree<Bot, State>.Node
                    {
                        Condition = bot => bot.allowFollow,
                        positive = new DecisionTree<Bot, State>.Node
                        {
                            decision = State.Follow
                        },
                        negative = new DecisionTree<Bot, State>.Node
                        {
                            decision = State.Idle
                        }
                    },
                    negative = new DecisionTree<Bot, State>.Node
                    {
                        Condition = bot =>
                            bot.followTarget != null
                            && (bot.followTarget.transform.position - bot.transform.position).magnitude >= maxFollowTargetDistance,
                        positive = new DecisionTree<Bot, State>.Node
                        {
                            decision = State.Follow
                        },
                        negative = new DecisionTree<Bot, State>.Node
                        {
                            decision = State.Attack
                        }
                    }
                },
                negative = new DecisionTree<Bot, State>.Node
                {
                    Condition = bot => bot.currentState == State.Idle,
                    positive = new DecisionTree<Bot, State>.Node
                    {
                        Condition = bot => bot.idleTimeLeft > 0,
                        positive = new DecisionTree<Bot, State>.Node
                        {
                            decision = State.Idle
                        },
                        negative = new DecisionTree<Bot, State>.Node
                        {
                            Condition = bot => bot.patrolType == PatrolType.Static,
                            positive = new DecisionTree<Bot, State>.Node
                            {
                                decision = State.Idle
                            },
                            negative = new DecisionTree<Bot, State>.Node
                            {
                                decision = State.Patrol
                            }
                        }
                    },
                    negative = new DecisionTree<Bot, State>.Node
                    {
                        Condition = bot => bot.currentState == State.Patrol,
                        positive = new DecisionTree<Bot, State>.Node
                        {
                            Condition = bot => bot.idleOnlyOnTargets,
                            positive = new DecisionTree<Bot, State>.Node
                            {
                                Condition = bot => (bot.patrolTargets[currentPatrolTarget] - bot.transform.position).magnitude <= Bot.minPatrolStoppingDistance,
                                positive = new DecisionTree<Bot, State>.Node
                                {
                                    decision = State.Idle
                                },
                                negative = new DecisionTree<Bot, State>.Node
                                {
                                    decision = State.Patrol
                                }
                            },
                            negative = new DecisionTree<Bot, State>.Node
                            {
                                Condition = bot => bot.patrolTimeLeft > 0,
                                positive = new DecisionTree<Bot, State>.Node
                                {
                                    decision = State.Patrol
                                },
                                negative = new DecisionTree<Bot, State>.Node
                                {
                                    decision = State.Idle
                                }
                            }
                        },
                        negative = new DecisionTree<Bot, State>.Node
                        {
                            decision = State.Idle
                        }
                    }
                }
            }
        };

    }

    void OnDrawGizmos()
    {
        if(showPatrolTargets && patrolTargets != null)
        {
            Gizmos.color = Color.yellow;
            for(int i = 0; i < patrolTargets.Length; i++)
            {
                Vector3 p = patrolTargets[i];

                Gizmos.DrawLine(p - Vector3.left * 0.5f, p + Vector3.left * 0.5f);
                Gizmos.DrawLine(p - Vector3.up * 0.5f, p + Vector3.up * 0.5f);
            }
        }

        if (showEnemyDetectionInfo && enemyDetectionRadius > 0)
        {
            if (!faceController)
                faceController = GetComponent<FaceController>();

            Gizmos.color = Color.red;
            if (!enableFieldOfView)
            {
                Gizmos.DrawWireSphere(transform.position, enemyDetectionRadius);
            }
            else
            {

                Vector3 rayOrigin = transform.position;
                float angle = fieldOfViewAngle * 0.5f * Mathf.Deg2Rad;
                Vector2 fDir = faceController.GetFaceDirection();

                Vector2 minusFOV = new Vector2(
                    fDir.x * Mathf.Cos(-angle ) + fDir.y * Mathf.Sin(-angle),
                    -fDir.x * Mathf.Sin(-angle) + fDir.y * Mathf.Cos(-angle)
                );
                Vector2 plusFOV = new Vector2(
                    fDir.x * Mathf.Cos(angle) + fDir.y * Mathf.Sin(angle),
                    -fDir.x * Mathf.Sin(angle) + fDir.y * Mathf.Cos(angle)
                );
                Gizmos.DrawRay(rayOrigin, minusFOV * enemyDetectionRadius);
                Gizmos.DrawRay(rayOrigin, plusFOV * enemyDetectionRadius);

                Gizmos.DrawWireSphere(transform.position, enemyHearRadius);
            }
        }

        if(showEnemyLink && targetInRange && !targetIsFriendly)
        {
            //Check if target is behind obstacle
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
        
        if(showFriendDetectionRadius && friendDetectionRadius > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, friendDetectionRadius);
        }

        if (showFrindlyLink && targetInRange && targetIsFriendly)
        {
            //Check if target is behind obstacle
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, followTarget.transform.position);
        }
    }

    public enum PatrolType
    {
        Static,
        Preset,
        Random
    }

    public enum State
    {
        Idle,
        Patrol,
        Attack,
        Follow
    }

    public enum Disposition
    {
        Aggressive,
        Passive,
        Defensive
    }

    public class DecisionTree<Client, Result>
    {
        public Node root;

        public Result Evaluate(Client client, bool showDebugInfo)
        {
            if(showDebugInfo)
                Debug.Log("START DECISION");
            Result res = root.Evaluate(client, showDebugInfo);
            if (showDebugInfo)
                Debug.Log("END DECISION");

            return res;
        }

        public class Node
        {
            public Node positive;
            public Node negative;

            public Result decision;
            public Func<Client, bool> Condition;

            public Result Evaluate(Client client, bool showDebugInfo)
            {
                if(Condition == null)
                {
                    if(decision == null)
                    {
                        Debug.LogWarning("DecisionTree LeafNode doesn't have a descision.");
                        return default(Result);
                    } else
                    {
                        return decision;
                    }
                }
                bool res = Condition(client);

                if(res)
                {
                    if(positive == null)
                    {
                        Debug.LogWarning("DecisionTree incomplete.");
                        return default(Result);
                    }

                    if (showDebugInfo)
                        Debug.Log("Going Positive");
                    return positive.Evaluate(client, showDebugInfo);
                } else
                {
                    if(negative == null)
                    {
                        Debug.LogWarning("DecisionTree incomplete.");
                        return default(Result);
                    }
                    if (showDebugInfo)
                        Debug.Log("Going Negative");
                    return negative.Evaluate(client,showDebugInfo);
                }
            }
        }
    }
}
