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
    
    [Header("Настройки анимации")]
    public Animator dynamiteAnimator;
    public string fuseAnimationTrigger = "Fuse";
    
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
        }

        if (playerLayer == 0)
            playerLayer = LayerMask.GetMask("Player");

        if (!isHeld && !isPlaced)
            Invoke(nameof(Disappear), disappearIfNotPickedUp);
            
        if (dynamiteAnimator == null)
            dynamiteAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isHeld)
        {
            if (holdPoint != null)
            {
                transform.position = holdPoint.position;
            }
        }
        else if (!isPlaced)
        {
            CheckPickUp();
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

        if (playerController != null)
        {
            if (playerController.HasDynamite())
                return;

            holdPoint = playerController.GetHoldPoint();

            if (holdPoint == null)
                return;

            isHeld = true;

            dynamiteCollider.enabled = false;
            rb.simulated = false;

            CancelInvoke(nameof(Disappear));
            CancelInvoke(nameof(Explode));

            transform.SetParent(holdPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            playerController.SetDynamite(gameObject);
        }
    }

    void PlaceDynamite()
    {
        isHeld = false;
        isPlaced = true;

        transform.SetParent(null);

        dynamiteCollider.enabled = true;
        rb.simulated = true;

        CancelInvoke(nameof(Disappear));
        
        if (dynamiteAnimator != null && !string.IsNullOrEmpty(fuseAnimationTrigger))
        {
            dynamiteAnimator.SetTrigger(fuseAnimationTrigger);
        }
        
        Invoke(nameof(Explode), fuseTime);

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
            pc.RemoveDynamite();
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