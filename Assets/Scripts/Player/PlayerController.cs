using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;
    public Transform holdPoint;
    
    private Rigidbody2D rb;
    private GameObject currentDynamite;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
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
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * Speed;

        //поворот обьекта!!
        if (moveX != 0)
        {
            transform.rotation = Quaternion.Euler(0, moveX > 0 ? 0 : 180, 0);
        }

        // Проверка на установку динамита
        if (Input.GetKeyDown(KeyCode.E) && currentDynamite != null)
        {
            Dynamite dyn = currentDynamite.GetComponent<Dynamite>();
            if (dyn != null)
            {
                dyn.Place();
            }
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