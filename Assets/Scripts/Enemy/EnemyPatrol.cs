using System.Collections;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float patrolRadius = 5f;
    public float obstacleDetectDistance = 1f; // distance to detect walls
    public LayerMask wallLayer;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float detectionRadius = 7f;
    public float loseSightRadius = 10f;
    public Transform player;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float giveDamage = 5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float waitTime;
    private bool isWaiting;
    private bool isChasing;
    private bool isAttacking;
    private float attackTimer;

    private Animator animator;
    private bool hasTriggeredChaseWorld = false;
    private bool isPlayerDetected = false;
    private bool hasSeenPlayer = false;
    private bool chaseStarted = false;

    private Vector3 lastKnownPlayerPos;
    private bool isSearching = false;
    private float searchDuration = 3f;
    private float searchTimer;

    void Start()
    {
        startPos = transform.position;
        animator = GetComponent<Animator>();
        SetNewTarget();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Freeze during initial camera shake
        if (isPlayerDetected && !chaseStarted)
            return;

        // Detect player
        if (!isChasing && distanceToPlayer <= detectionRadius)
        {
            StartChasing();
        }

        // Chasing logic
        if (isChasing)
        {
            if (distanceToPlayer <= attackRange)
            {
                StartAttack();
            }
            else if (distanceToPlayer <= loseSightRadius)
            {
                ChasePlayer();
                lastKnownPlayerPos = player.position; // update last known position
                isSearching = false;
            }
            else
            {
                // Player lost, start searching
                if (!isSearching)
                {
                    isSearching = true;
                    searchTimer = searchDuration;
                }
                SearchForPlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    // --- PATROL LOGIC ---
    void Patrol()
    {
        animator.SetBool("isRunning", false);

        if (isWaiting)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                SetNewTarget();
            }
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void SetNewTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        targetPos = startPos + new Vector3(randomPoint.x, 0, randomPoint.y);
        isWaiting = false;
        animator.SetBool("isWalking", true);
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPos - transform.position).normalized;

        // Check for walls in front
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, obstacleDetectDistance, wallLayer))
        {
            // Pick a new random patrol target if hitting obstacle
            SetNewTarget();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            isWaiting = true;
            waitTime = Random.Range(minWaitTime, maxWaitTime);
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (direction != Vector3.zero)
                transform.forward = direction;
        }
    }

    // --- CHASE LOGIC ---
    void StartChasing()
    {
        isChasing = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
    }

    void ChasePlayer()
    {
        if (player == null) return;

        animator.SetBool("isRunning", true);
        animator.SetBool("isWalking", false);

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.forward = direction;

        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        if (!hasTriggeredChaseWorld)
        {
            hasTriggeredChaseWorld = true;
            //FindAnyObjectByType<CameraShake>()?.StartShake(0.5f, 0.2f);
            //FindAnyObjectByType<ChaseWorldManager>()?.GenerateChaseWorld();
        }
    }

    void SearchForPlayer()
    {
        animator.SetBool("isRunning", true);

        // Move toward last known player position
        transform.position = Vector3.MoveTowards(transform.position, lastKnownPlayerPos, moveSpeed * Time.deltaTime);

        Vector3 dir = (lastKnownPlayerPos - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.forward = dir;

        searchTimer -= Time.deltaTime;

        // After search duration, return to patrol
        if (searchTimer <= 0)
        {
            isChasing = false;
            isSearching = false;
            animator.SetBool("isRunning", false);
            SetNewTarget();
        }
    }

    // --- ATTACK LOGIC ---
    void StartAttack()
    {
        if (attackTimer > 0) return;

        attackTimer = attackCooldown;
        animator.SetTrigger("Attack");

        isAttacking = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir;

        Invoke(nameof(ResetAttackState), 1f);
    }

    public void DealDamageToPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            playerHealth playerHealth = player.GetComponent<playerHealth>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(giveDamage);
                Debug.Log("Player took damage!");
            }
        }
    }

    void ResetAttackState()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasSeenPlayer = true;
            isPlayerDetected = true;

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);

            if (!hasTriggeredChaseWorld)
            {
                hasTriggeredChaseWorld = true;
                //FindAnyObjectByType<CameraShake>()?.StartShake(4f, 0.05f);
                StartCoroutine(StartChaseAfterDelay(4f));
            }
        }
    }

    IEnumerator StartChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        chaseStarted = true;
        hasTriggeredChaseWorld = true;
        animator.SetBool("isRunning", true);
    }
}
