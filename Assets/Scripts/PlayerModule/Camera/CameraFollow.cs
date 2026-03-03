using UnityEngine;

namespace PlayerModule.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform Player; 
        public float smooth = 5f; 
    
        void LateUpdate()
        {
            if (Player == null) return;
        
            Vector3 targetPosition = new Vector3(Player.position.x, Player.position.y, transform.position.z);
        
            transform.position = Vector3.Lerp(transform.position, targetPosition, smooth * Time.deltaTime);
        }
    }
}
