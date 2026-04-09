using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Lamp : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Максимальная дистанция обнаружения символа")]
    public float detectionRange = 3f;

    [Tooltip("Угол луча (в градусах) — чем меньше, тем точнее нужно наводить")]
    public float detectionAngle = 20f;

    [Tooltip("Слой, на котором находятся скрытые символы")]
    public LayerMask symbolLayer;

    [Header("Materials")]
    [Tooltip("Материал символа в обычном (невидимом) состоянии")]
    public Material invisibleMaterial;

    [Tooltip("Материал символа в УФ-режиме")]
    public Material visibleMaterial;

    // Приватные переменные
    private bool isHeld = false;          // В руках ли фонарик
    private GameObject currentDetectedSymbol = null;
    private Renderer currentRenderer = null;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        // Находим компонент захвата для VR
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Подписываемся на события взятия и отпускания
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        // Проверяем, что все материалы назначены
        if (invisibleMaterial == null || visibleMaterial == null)
        {
            Debug.LogError("UVFlashlightDetector: Не назначены материалы для символов!");
        }
    }

    void Update()
    {
        // Работаем ТОЛЬКО когда фонарик в руках
        if (!isHeld) return;

        // Точка, из которой выходит луч (центр фонарика)
        Vector3 rayOrigin = transform.position;

        // Если есть дочерний объект BeamOrigin — используем его (опционально)
        Transform beamOrigin = transform.Find("BeamOrigin");
        if (beamOrigin != null)
            rayOrigin = beamOrigin.position;

        Ray ray = new Ray(rayOrigin, transform.forward);
        RaycastHit hit;

        // Визуализация луча в Scene View (только для отладки)
        Debug.DrawRay(ray.origin, ray.direction * detectionRange, Color.magenta);

        if (Physics.Raycast(ray, out hit, detectionRange, symbolLayer))
        {
            // Проверяем угол между направлением фонарика и направлением на символ
            Vector3 directionToHit = (hit.point - rayOrigin).normalized;
            float angle = Vector3.Angle(transform.forward, directionToHit);

            if (angle <= detectionAngle)
            {
                // Нашли символ в зоне луча
                RevealSymbol(hit.collider.gameObject);
            }
            else
            {
                HideSymbol();
            }
        }
        else
        {
            HideSymbol();
        }
    }

    void RevealSymbol(GameObject symbol)
    {
        // Если это тот же символ — ничего не делаем
        if (currentDetectedSymbol == symbol) return;

        // Прячем предыдущий символ
        HideSymbol();

        // Показываем новый
        currentDetectedSymbol = symbol;
        currentRenderer = symbol.GetComponent<Renderer>();

        if (currentRenderer != null)
        {
            // Меняем на видимый материал
            currentRenderer.material = visibleMaterial;
        }
    }

    void HideSymbol()
    {
        if (currentDetectedSymbol != null && currentRenderer != null)
        {
            // Возвращаем невидимый материал
            currentRenderer.material = invisibleMaterial;
        }

        currentDetectedSymbol = null;
        currentRenderer = null;
    }

    // Фонарик взяли в руки
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
        Debug.Log("UV Flashlight: ON (в руках)");
    }

    // Фонарик отпустили
    void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;

        // Прячем подсвеченный символ, если он был
        HideSymbol();

        Debug.Log("UV Flashlight: OFF (отпущен)");
    }

    void OnDestroy()
    {
        // Отписываемся от событий, чтобы избежать утечек памяти
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}