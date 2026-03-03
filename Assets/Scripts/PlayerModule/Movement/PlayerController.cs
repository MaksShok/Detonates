using UnityEngine;

namespace PlayerModule.Movement
{
    public class PlayerController : MonoBehaviour
    {
        public float Speed = 5f;
        private Rigidbody2D rb;
    
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        
            rb.gravityScale = 0f;        
            rb.freezeRotation = true;    
            rb.linearDamping = 10f;      
        }
    
        void Update()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
        
            Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        
            rb.linearVelocity = moveDirection * Speed;
        }
    }
}