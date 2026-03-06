using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;
    public Transform holdPoint;
    
    [Header("Настройки анимации")]
    public Animator playerAnimator;
    public string walkAnimationParameter = "isWalking";
    public string directionXParameter = "moveX";
    public string directionYParameter = "moveY";
    public string attackTriggerParameter = "attack";
    
    [Header("Настройки атаки")]
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public float attackOffset = 1f;
    public float attackHeightOffset = 0.5f;
    
    private Rigidbody2D rb;
    private GameObject currentDynamite;
    private bool isAttacking = false;
    private float lastAttackTime;
    private Vector2 attackDirection;
    private Vector2 lastMoveDirection;
    private Camera mainCamera;
    private Vector3 originalAttackPointPosition;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        
        rb.gravityScale = 0f;        
        rb.freezeRotation = true;    
        rb.linearDamping = 10f;
        
        if (holdPoint == null)
        {
            GameObject hold = new GameObject("HoldPoint");
            hold.transform.parent = transform;
            hold.transform.localPosition = new Vector3(0.5f, 0.2f, 0);
            holdPoint = hold.transform;
        }
        
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
            
        if (attackPoint == null)
        {
            GameObject point = new GameObject("AttackPoint");
            point.transform.parent = transform;
            point.transform.localPosition = new Vector3(0, attackHeightOffset, 0);
            attackPoint = point.transform;
        }
        
        originalAttackPointPosition = attackPoint.localPosition;
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        
        if (!isAttacking)
        {
            Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
            rb.linearVelocity = moveDirection * Speed;
            
            if (moveX != 0 || moveY != 0)
            {
                lastMoveDirection = new Vector2(moveX, moveY);
            }
            
            if (attackPoint != null)
            {
                attackPoint.localPosition = originalAttackPointPosition;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        UpdateAnimation(moveX, moveY);
        
        if (Input.GetMouseButtonDown(0) && Time.time > lastAttackTime + attackCooldown && !isAttacking)
        {
            CalculateAttackDirection();
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.E) && currentDynamite != null)
        {
            Dynamite dyn = currentDynamite.GetComponent<Dynamite>();
            if (dyn != null)
            {
                dyn.Place();
            }
        }
    }
    
    void CalculateAttackDirection()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        attackDirection = (mousePos - transform.position).normalized;
        
        float x = attackDirection.x;
        float y = attackDirection.y;
        
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            attackDirection = new Vector2(x > 0 ? 1 : -1, 0);
        }
        else
        {
            attackDirection = new Vector2(0, y > 0 ? 1 : -1);
        }
    }
    
    void Attack()
{
    isAttacking = true;
    lastAttackTime = Time.time;
    
    if (attackPoint != null)
    {
        Vector3 offset = new Vector3(
            attackDirection.x * attackOffset,
            attackDirection.y * attackOffset + attackHeightOffset,
            0
        );
        attackPoint.position = transform.position + offset;
    }
    
    if (playerAnimator != null)
    {
        if (attackDirection.x > 0) 
        {
            playerAnimator.SetFloat(directionXParameter, 1f);
            playerAnimator.SetFloat(directionYParameter, 0f);
        }
        else if (attackDirection.x < 0) 
        {
            playerAnimator.SetFloat(directionXParameter, -1f);
            playerAnimator.SetFloat(directionYParameter, 0f);
        }
        else if (attackDirection.y > 0) 
        {
            playerAnimator.SetFloat(directionXParameter, 0f);
            playerAnimator.SetFloat(directionYParameter, 1f);
        }
        else if (attackDirection.y < 0) 
        {
            playerAnimator.SetFloat(directionXParameter, 0f);
            playerAnimator.SetFloat(directionYParameter, -1f);
        }
        
        playerAnimator.SetTrigger(attackTriggerParameter);
    }
    
    Invoke(nameof(AttackDamage), 0.2f);
    Invoke(nameof(EndAttack), 0.4f);
}
    
    void AttackDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
        
        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Попадание во врага: " + enemy.name);
        }
    }
    
    void EndAttack()
    {
        isAttacking = false;
    }
    
    void UpdateAnimation(float moveX, float moveY)
    {
        if (playerAnimator != null && !isAttacking)
        {
            bool isWalking = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f;
            
            playerAnimator.SetBool(walkAnimationParameter, isWalking);
            
            if (isWalking)
            {
                if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
                {
                    playerAnimator.SetFloat(directionXParameter, moveX);
                    playerAnimator.SetFloat(directionYParameter, 0f);
                }
                else
                {
                    playerAnimator.SetFloat(directionXParameter, 0f);
                    playerAnimator.SetFloat(directionYParameter, moveY);
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
    
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
    
    public bool HasDynamite()
    {
        return currentDynamite != null;
    }
    
    public void SetDynamite(GameObject dynamite)
    {
        currentDynamite = dynamite;
    }
    
    public void RemoveDynamite()
    {
        currentDynamite = null;
    }
}