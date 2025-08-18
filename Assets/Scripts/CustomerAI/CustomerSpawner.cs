using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private Transform deskTransform;
    [SerializeField] private Transform exitTransform;
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minSpawnTime = 1f; 
    [SerializeField] private float maxSpawnTime = 5f;
    
    private int maxCustomers;
    private float waitTime;
    private int currentCustomerCount = 0;
    private bool spawning = true;
    
    private IEnumerator Start()
    {
        while (ArcadeMachineService.Instance == null)
        {
            yield return null; 
        }
        StartCoroutine(SpawnCustomersCoroutine());
    }
    
    private IEnumerator SpawnCustomersCoroutine()
    {
        while (spawning)
        {
            maxCustomers = ArcadeMachineService.Instance.GetAvailableArcadeCount();
            if (currentCustomerCount < maxCustomers)
            {
                SpawnCustomer();
            }

            waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    private void SpawnCustomer()
    {
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

