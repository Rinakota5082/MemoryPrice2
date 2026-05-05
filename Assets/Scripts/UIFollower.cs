using UnityEngine;

public class UIFollower : MonoBehaviour
{
    public Transform target;        // за чем следить (обычно камера игрока)
    public float distance = 1.5f;   // расстояние от камеры
    public float heightOffset = 0f; // сдвиг по высоте

    void Start()
    {
        // Если target не задан, ищем камеру игрока
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Меню всегда перед камерой
            Vector3 targetPosition = target.position + target.forward * distance;
            targetPosition.y += heightOffset;
            transform.position = targetPosition;

            // Меню поворачивается к камере
            transform.LookAt(target);
        }
    }
}