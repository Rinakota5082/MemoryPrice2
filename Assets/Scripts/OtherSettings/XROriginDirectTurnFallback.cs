using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Hard fallback yaw rotation when XRI turn providers/actions are blocked.
/// Reads right-hand turn actions (and left as fallback for single-controller simulator).
/// </summary>
public sealed class XROriginDirectTurnFallback : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] string[] turnActionPaths =
    {
        "XRI Right Locomotion/Turn",
        "XRI Right Locomotion/Snap Turn",
        "XRI Left Locomotion/Turn",
        "XRI Left Locomotion/Snap Turn",
    };

    [Header("Rotation")]
    [SerializeField] float continuousTurnSpeed = 90f;
    [SerializeField] float snapTurnAmount = 45f;
    [SerializeField] float deadzone = 0.15f;
    [SerializeField] float snapCooldownSeconds = 0.35f;

    InputAction[] turnActions;
    float lastSnapTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoAttach()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        if (player.GetComponent<XROriginDirectTurnFallback>() == null)
            player.AddComponent<XROriginDirectTurnFallback>();
    }

    void OnEnable()
    {
        ResolveActions();
        EnableActions();
    }

    void OnDisable()
    {
        if (turnActions == null)
            return;

        foreach (var action in turnActions)
            action?.Disable();
    }

    void ResolveActions()
    {
        if (actions == null)
            actions = TryGetAnyLoadedInputActionAsset("XRI Right Locomotion");
        if (actions == null)
            return;

        turnActions = new InputAction[turnActionPaths.Length];
        for (var i = 0; i < turnActionPaths.Length; i++)
        {
            var path = turnActionPaths[i];
            turnActions[i] = actions.FindAction(path, false);
        }
    }

    void EnableActions()
    {
        if (turnActions == null)
            return;

        foreach (var action in turnActions)
        {
            if (action == null)
                continue;

            action.Enable();
            if (action.actionMap != null && !action.actionMap.enabled)
                action.actionMap.Enable();
            if (action.actionMap?.asset != null && !action.actionMap.asset.enabled)
                action.actionMap.asset.Enable();
        }
    }

    void Update()
    {
        if (turnActions == null || turnActions.Length == 0)
        {
            ResolveActions();
            EnableActions();
            if (turnActions == null || turnActions.Length == 0)
                return;
        }

        var input = ReadBestTurnInput(out var isSnapTurnAction);
        if (Mathf.Abs(input.x) < deadzone)
            return;

        if (isSnapTurnAction)
            ApplySnapTurn(input.x);
        else
            transform.Rotate(0f, input.x * continuousTurnSpeed * Time.deltaTime, 0f, Space.World);
    }

    Vector2 ReadBestTurnInput(out bool isSnapTurnAction)
    {
        isSnapTurnAction = false;
        var best = Vector2.zero;

        for (var i = 0; i < turnActions.Length; i++)
        {
            var action = turnActions[i];
            if (action == null)
                continue;

            var value = action.ReadValue<Vector2>();
            if (value.sqrMagnitude <= best.sqrMagnitude)
                continue;

            best = value;
            var path = turnActionPaths[i];
            isSnapTurnAction = path.Contains("Snap Turn");
        }

        return best;
    }

    void ApplySnapTurn(float axis)
    {
        if (Time.unscaledTime - lastSnapTime < snapCooldownSeconds)
            return;

        if (Mathf.Abs(axis) < deadzone)
            return;

        var yaw = axis > 0f ? snapTurnAmount : -snapTurnAmount;
        transform.Rotate(0f, yaw, 0f, Space.World);
        lastSnapTime = Time.unscaledTime;
    }

    static InputActionAsset TryGetAnyLoadedInputActionAsset(string requiredMapName)
    {
        var all = Resources.FindObjectsOfTypeAll<InputActionAsset>();
        if (all == null)
            return null;

        foreach (var asset in all)
        {
            if (asset == null)
                continue;
            if (asset.FindActionMap(requiredMapName, false) != null)
                return asset;
        }

        return null;
    }
}
