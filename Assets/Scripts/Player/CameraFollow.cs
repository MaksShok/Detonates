using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Объекты")]
    public Transform player; // Персонаж
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Смещение камеры

    [Header("Барьер КАМЕРЫ (где она останавливается первой)")]
    public float camMinX = -40f, camMaxX = 40f;
    public float camMinY = -30f, camMaxY = 30f;

    [Header("Настройки плавности")]
    public float followSpeed = 5f; // Насколько быстро камера "догоняет" персонажа

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        // 2. Вычисляем, где камера ХОТЕЛА бы быть (следом за игроком)
        Vector3 desiredCamPos = player.position + offset;

        // 3. Камера "врезается" в свой барьер и останавливается
        desiredCamPos.x = Mathf.Clamp(desiredCamPos.x, camMinX, camMaxX);
        desiredCamPos.y = Mathf.Clamp(desiredCamPos.y, camMinY, camMaxY);

        // 4. Плавно двигаем камеру к этой точке (эффект "отлипания" и следования)
        transform.position = Vector3.SmoothDamp(transform.position, desiredCamPos, ref velocity, 1f / followSpeed);
    }
}
