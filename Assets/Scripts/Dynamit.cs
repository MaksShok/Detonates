using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public SpawnerDynamit spawner;
    
    [Header("Таймеры")]
    public float disappearIfNotPickedUp = 10f; 
    public float fuseTime = 3f;                
    
    [Header("Настройки подбора")]
    public float pickUpRange = 1.5f;
    public LayerMask playerLayer;
    
    [Header("Настройки взрыва")]
    public float explosionRadius = 3f;
    public float explosionDamage = 100f;
    public GameObject explosionEffect;
    
    private bool isHeld = false;
    private bool isPlaced = false;
    private Transform holdPoint;
    private GameObject player;
    private Collider2D dynamiteCollider;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        dynamiteCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 5f;
        }

        if (playerLayer == 0)
            playerLayer = LayerMask.GetMask("Player");

        // Исчезновение динамита если он просто лежит
        if (!isHeld && !isPlaced)
            Invoke(nameof(Disappear), disappearIfNotPickedUp);
    }

    void Update()
    {
        if (isHeld)
        {
            FollowPlayer();
        }
        else if (!isPlaced)
        {
            CheckPickUp();
        }
    }

    void FollowPlayer()
    {
        if (holdPoint != null)
        {
            transform.position = Vector3.Lerp(transform.position, holdPoint.position, Time.deltaTime * 15f);
        }
    }

    void CheckPickUp()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpRange, playerLayer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                player = collider.gameObject;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpDynamite();
                }
                break;
            }
        }
    }

    void PickUpDynamite()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();

        // проверка есть динамит или нет
        if (playerController != null)
        {
            if (playerController.HasDynamite())
            {
                return;
            }

            holdPoint = playerController.GetHoldPoint();

            if (holdPoint == null)
            {
                return;
            }

            isHeld = true;

            if (dynamiteCollider != null)
                dynamiteCollider.enabled = false;

            if (rb != null)
                rb.simulated = false;

            CancelInvoke(nameof(Disappear));
            CancelInvoke(nameof(Explode));

            playerController.SetDynamite(gameObject);

            transform.parent = player.transform;

            Debug.Log("Динамит подобран!");
        }
    }

    void PlaceDynamite()
    {
        isHeld = false;
        isPlaced = true;

        transform.parent = null;

        
        if (dynamiteCollider != null)
            dynamiteCollider.enabled = true;

        if (rb != null)
            rb.simulated = true;

        // тут я отменяю взрыв
        CancelInvoke(nameof(Disappear));

        Invoke(nameof(Explode), fuseTime);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;

        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.RemoveDynamite();
        }
    }

    public void Place()
    {
        if (isHeld)
        {
            PlaceDynamite();
        }
    }

    void Disappear()
    {
        if (isHeld || isPlaced)
            return;

        if (spawner != null)
            spawner.OnDynamiteRemoved();

        Destroy(gameObject);
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (spawner != null)
            spawner.OnDynamiteRemoved();

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }
}