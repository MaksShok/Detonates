using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public SpawnerDynamit spawner;
    public float fuseTime = 3f;

    
    void Start()
    {
        Invoke("Explode", fuseTime);
    }
    
    void Explode()
    {   
        if (spawner != null)
            spawner.OnDynamiteRemoved();
            
        Destroy(gameObject);
    }
}