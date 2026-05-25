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

    private bool isHeld = false;          
    private GameObject currentDetectedSymbol = null;
    private Renderer currentRenderer = null;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Подписываемся на события взятия и отпускания
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void Update()
    {
        // Работаем ТОЛЬКО когда фонарик в руках
        if (!isHeld) return;
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
        if (currentDetectedSymbol == symbol) return;
        HideSymbol();
        currentDetectedSymbol = symbol;
        currentRenderer = symbol.GetComponent<Renderer>();

        if (currentRenderer != null)
        {
            currentRenderer.material = visibleMaterial;
        }
    }

    void HideSymbol()
    {
        if (currentDetectedSymbol != null && currentRenderer != null)
        {
            currentRenderer.material = invisibleMaterial;
        }

        currentDetectedSymbol = null;
        currentRenderer = null;
    }
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
    }
    void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
        HideSymbol();
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