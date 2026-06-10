using UnityEngine;
using UnityEngine.AI;

public class EnemyChopTree : MonoBehaviour
{
    public float stopDistance = 4f;
    public int chopDamage = 10;
    public float chopInterval = 1.5f;

    private Transform targetTree;
    private TreeHealth treeHealth;
    private NavMeshAgent agent;
    private Animator animator;
    private float chopTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.stoppingDistance = stopDistance;
        FindNearestTree();
    }

    void Update()
    {
        if (targetTree == null || treeHealth == null || treeHealth.IsDead)
        {
            ClearTarget();
            FindNearestTree();
            return;
        }

        bool reachedTree =
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.5f;

        if (!reachedTree)
        {
            agent.isStopped = false;
            agent.SetDestination(targetTree.position);

            if (animator != null)
                animator.SetBool("IsWalking", true);
        }
        else
        {
            agent.isStopped = true;

            if (animator != null)
                animator.SetBool("IsWalking", false);

            LookAtTree();

            chopTimer += Time.deltaTime;

            if (chopTimer >= chopInterval)
            {
                ChopTree();
                chopTimer = 0f;
            }
        }
    }

    void ChopTree()
    {
        if (treeHealth == null || targetTree == null || treeHealth.IsDead)
        {
            ClearTarget();
            FindNearestTree();
            return;
        }

        if (animator != null)
            animator.SetTrigger("Chop");

        bool treeDied = treeHealth.TakeDamage(chopDamage);

        if (treeDied)
        {
            ClearTarget();
            FindNearestTree();
        }
    }

    void ClearTarget()
    {
        targetTree = null;
        treeHealth = null;
        chopTimer = 0f;

        if (animator != null)
        {
            animator.ResetTrigger("Chop");
            animator.SetBool("IsWalking", false);
        }
    }

    void LookAtTree()
    {
        if (targetTree == null) return;

        Vector3 dir = targetTree.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void FindNearestTree()
    {
        TreeHealth[] trees = FindObjectsOfType<TreeHealth>();

        TreeHealth nearestTree = null;
        float nearestDistance = Mathf.Infinity;

        foreach (TreeHealth tree in trees)
        {
            if (tree == null || tree.IsDead) continue;

            float distance = Vector3.Distance(transform.position, tree.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTree = tree;
            }
        }

        if (nearestTree == null)
        {
            Debug.Log("Semua pohon sudah habis!");

            if (agent != null)
                agent.isStopped = true;

            if (animator != null)
                animator.SetBool("IsWalking", false);

            return;
        }

        targetTree = nearestTree.transform;
        treeHealth = nearestTree;
        chopTimer = 0f;

        agent.isStopped = false;
        agent.SetDestination(targetTree.position);

        Debug.Log("Target pohon baru: " + targetTree.name);
    }
}