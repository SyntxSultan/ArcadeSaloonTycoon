using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private Transform deskTransform;
    [SerializeField] private Transform exitTransform;
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxCustomers = 5;
    
    private int currentCustomerCount = 0;
    
    private void Start()
    {
        InvokeRepeating(nameof(SpawnCustomer), 1f, spawnInterval);
    }
    
    private void SpawnCustomer()
    {
        maxCustomers = ArcadeMachineService.Instance.GetAvailableArcadeCount();
        if (currentCustomerCount >= maxCustomers) return;
        
        GameObject customer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        CustomerAI customerAI = customer.GetComponent<CustomerAI>();
        customerAI.SetTransforms(deskTransform, exitTransform);
        customerAI.OnCustomerCompleted += OnCustomerCompleted;
        
        currentCustomerCount++;
    }
    
    private void OnCustomerCompleted(CustomerAI customer)
    {
        currentCustomerCount--;
    }
}

