using UnityEngine;

public class SpawnerDynamit : MonoBehaviour
{
    public GameObject dynamitePrefab;
    public Transform statue;
    
    [Header("Настройки спавна")]
    public float spawnInterval = 5f;
    public float spawnDistance = 3f;
    public float spawnYOffset = -0.5f; 
    public int maxDynamite = 1;
    
    private float nextSpawnTime;
    private int currentDynamite = 0;
    
    void Start()
    {
        if (statue == null)
            statue = transform;
            
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    void Update()
    {
        if (Time.time >= nextSpawnTime && currentDynamite < maxDynamite)
        {
            SpawnDynamite();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    void SpawnDynamite()
    {
        if (dynamitePrefab == null) return;
        
        int side = Random.Range(0, 2);
        float xOffset = (side == 0) ? -spawnDistance : spawnDistance;
        Vector3 spawnPos = statue.position + new Vector3(xOffset, spawnYOffset, 0);
        
        GameObject dynamite = Instantiate(dynamitePrefab, spawnPos, Quaternion.identity);
        
        Dynamite dynamiteScript = dynamite.GetComponent<Dynamite>();
        if (dynamiteScript == null)
            dynamiteScript = dynamite.AddComponent<Dynamite>();
        
        dynamiteScript.spawner = this;
        
        currentDynamite++;
    }
    
    public void OnDynamiteRemoved()
    {
        currentDynamite--;
        if (currentDynamite < 0) currentDynamite = 0;
    }
    
    void OnDrawGizmosSelected()
    {
        if (statue == null) return;
        
        Gizmos.color = Color.red;
        Vector3 leftPos = statue.position + new Vector3(-spawnDistance, spawnYOffset, 0);
        Vector3 rightPos = statue.position + new Vector3(spawnDistance, spawnYOffset, 0);
        
        Gizmos.DrawWireSphere(leftPos, 0.3f);
        Gizmos.DrawWireSphere(rightPos, 0.3f);
        Gizmos.DrawLine(leftPos, rightPos);
    }
}