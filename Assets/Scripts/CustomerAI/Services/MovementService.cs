using UnityEngine;
using UnityEngine.AI;

public class MovementService : MonoBehaviour, IMovementService
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;
    
    private NavMeshAgent navMeshAgent;
    private Transform currentTarget;
    private System.Action onReachedCallback;
    
    public bool IsMoving => navMeshAgent != null && navMeshAgent.pathPending || navMeshAgent.remainingDistance > stoppingDistance;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is required for MovementService!");
            return;
        }
        
        // Configure NavMeshAgent
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.updateRotation = false; // We'll handle rotation manually for smooth facing
    }
    
    public void MoveToTarget(Transform target, System.Action onReached)
    {
        if (navMeshAgent == null || target == null) return;
        
        currentTarget = target;
        onReachedCallback = onReached;
        
        navMeshAgent.SetDestination(target.position);
    }
    
    public void Stop()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
        }
        
        currentTarget = null;
        onReachedCallback = null;
    }
    
    private void Update()
    {
        if (navMeshAgent == null) return;
        
        // Handle rotation to face movement direction
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        // Face target when stopped and have a target
        else if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            directionToTarget.y = 0; // Keep rotation only on Y axis
            
            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        // Check if destination reached
        if (currentTarget != null && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= stoppingDistance)
        {
            var callback = onReachedCallback;
            currentTarget = null;
            onReachedCallback = null;
            callback?.Invoke();
        }
    }
    
    private void OnValidate()
    {
        // Update NavMeshAgent properties when values change in inspector
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }
    }
}

/*
public class MovementService : MonoBehaviour, IMovementService
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;
    
    private Transform currentTarget;
    private System.Action onReachedCallback;
    
    public bool IsMoving { get; private set; }
    
    public void MoveToTarget(Transform target, System.Action onReached)
    {
        currentTarget = target;
        onReachedCallback = onReached;
        IsMoving = true;
    }
    
    public void Stop()
    {
        IsMoving = false;
        currentTarget = null;
        onReachedCallback = null;
    }
    
    private void Update()
    {
        if (IsMoving && currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            
            if (Vector3.Distance(transform.position, currentTarget.position) <= stoppingDistance)
            {
                IsMoving = false;
                onReachedCallback?.Invoke();
                onReachedCallback = null;
            }
        }
    }
}
*/