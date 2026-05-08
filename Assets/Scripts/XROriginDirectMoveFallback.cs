using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Hard fallback movement for XR Origin when XRI locomotion pipeline gets blocked.
/// Reads "XRI Left Locomotion/Move" and moves the CharacterController directly.
/// </summary>
public sealed class XROriginDirectMoveFallback : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionAsset actions;
    [SerializeField] string actionMapName = "XRI Left Locomotion";
    [SerializeField] string moveActionName = "Move";

    [Header("Movement")]
    [SerializeField] float speed = 2.5f;
    [SerializeField] Transform forwardSource; // usually XR Origin Camera
    [SerializeField] bool debugLogs = true;
    [Tooltip("If true, moves Transform directly (ignores CharacterController collisions).")]
    [SerializeField] bool forceTransformMove;
    [Tooltip("If true, temporarily disables CharacterController collisions while moving.")]
    [SerializeField] bool disableCharacterControllerCollisions;
    [Tooltip("If movement is blocked (CC reports Sides and actual delta ~0), automatically bypass with Transform move.")]
    [SerializeField] bool autoBypassWhenStuck = true;
    [SerializeField] int stuckFramesBeforeBypass = 5;

    int stuckFrames;

    CharacterController cc;
    InputAction moveAction;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (forwardSource == null)
        {
            // Best-effort camera lookup
            var cam = GetComponentInChildren<Camera>(true);
            if (cam != null)
                forwardSource = cam.transform;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoAttach()
    {
        // Best-effort: attach to the Player-tagged XR Origin if it exists.
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        if (player.GetComponent<CharacterController>() == null)
            return;

        if (player.GetComponent<XROriginDirectMoveFallback>() == null)
            player.AddComponent<XROriginDirectMoveFallback>();
    }

    void OnEnable()
    {
        ResolveAction();
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
    }

    void ResolveAction()
    {
        if (actions == null)
            actions = TryGetActionsFromAnyInputActionManager(actionMapName);
        if (actions == null)
            actions = TryGetAnyLoadedInputActionAsset(actionMapName);

        if (actions == null)
            return;

        var action = actions.FindAction($"{actionMapName}/{moveActionName}", false);
        moveAction = action;

        if (moveAction == null)
        {
            // Try the right-hand locomotion map too (simulator often flips hands).
            var altMap = actionMapName == "XRI Left Locomotion" ? "XRI Right Locomotion" : "XRI Left Locomotion";
            var alt = actions.FindAction($"{altMap}/{moveActionName}", false);
            if (alt != null)
            {
                actionMapName = altMap;
                moveAction = alt;
            }
        }

        if (debugLogs)
        {
            if (actions == null)
                Debug.LogWarning($"[{nameof(XROriginDirectMoveFallback)}] No InputActionAsset found.", this);
            else if (moveAction == null)
                Debug.LogWarning($"[{nameof(XROriginDirectMoveFallback)}] Can't find action '{actionMapName}/{moveActionName}' in asset '{actions.name}'.", this);
            else
                Debug.Log($"[{nameof(XROriginDirectMoveFallback)}] Using action '{actionMapName}/{moveActionName}' from '{actions.name}'.", this);
        }
    }

    void Update()
    {
        if (cc == null)
            return;

        if (moveAction == null)
        {
            ResolveAction();
            if (moveAction == null)
                return;
            moveAction.Enable();
        }

        var input = moveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude < 0.0001f)
            return;

        var src = forwardSource != null ? forwardSource : transform;
        var forward = src.forward;
        forward.y = 0f;
        forward.Normalize();
        var right = src.right;
        right.y = 0f;
        right.Normalize();

        var desired = (forward * input.y + right * input.x) * (speed * Time.deltaTime);
        var before = transform.position;

        if (forceTransformMove)
        {
            transform.position = before + desired;
            if (debugLogs)
                Debug.Log($"[{nameof(XROriginDirectMoveFallback)}] TRANSFORM move input={input} desired={desired} pos {before} -> {transform.position}", this);
            return;
        }

        var prevDetect = cc.detectCollisions;
        if (disableCharacterControllerCollisions)
            cc.detectCollisions = false;

        var flags = cc.Move(desired);

        if (disableCharacterControllerCollisions)
            cc.detectCollisions = prevDetect;

        var after = transform.position;
        var actual = after - before;

        if (autoBypassWhenStuck && desired.sqrMagnitude > 0.000001f)
        {
            var blocked = actual.sqrMagnitude < 0.00000001f && (flags & CollisionFlags.Sides) != 0;
            if (blocked)
                stuckFrames++;
            else
                stuckFrames = 0;

            if (stuckFrames >= stuckFramesBeforeBypass)
            {
                stuckFrames = 0;
                transform.position = before + desired;
                if (debugLogs)
                    Debug.LogWarning($"[{nameof(XROriginDirectMoveFallback)}] CC blocked (Sides). Auto-bypassing with TRANSFORM move this frame.", this);
                return;
            }
        }

        if (debugLogs)
            Debug.Log($"[{nameof(XROriginDirectMoveFallback)}] CC move input={input} desired={desired} actual={actual} flags={flags} pos {before} -> {after}", this);
    }

    static InputActionAsset TryGetActionsFromAnyInputActionManager(string requiredMapName)
    {
        // Find all behaviours (also inactive) and look for a component named "InputActionManager"
        // with an "actionAssets" property/field containing InputActionAsset[].
        var all = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        foreach (var b in all)
        {
            if (b == null)
                continue;

            var t = b.GetType();
            if (t.Name != "InputActionManager")
                continue;

            // property: actionAssets
            var prop = t.GetProperty("actionAssets", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (prop != null && prop.PropertyType.IsArray && prop.PropertyType.GetElementType() == typeof(InputActionAsset))
            {
                if (prop.GetValue(b) is InputActionAsset[] assets)
                {
                    var found = FirstAssetWithMap(assets, requiredMapName);
                    if (found != null)
                        return found;
                }
            }

            // backing field: m_ActionAssets
            var field = t.GetField("m_ActionAssets", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null && field.FieldType.IsArray && field.FieldType.GetElementType() == typeof(InputActionAsset))
            {
                if (field.GetValue(b) is InputActionAsset[] assets)
                {
                    var found = FirstAssetWithMap(assets, requiredMapName);
                    if (found != null)
                        return found;
                }
            }
        }

        return null;
    }

    static InputActionAsset TryGetAnyLoadedInputActionAsset(string requiredMapName)
    {
        // If there's no InputActionManager, still try any loaded assets (scene may reference it).
        var all = Resources.FindObjectsOfTypeAll<InputActionAsset>();
        if (all == null)
            return null;

        foreach (var a in all)
        {
            if (a == null)
                continue;
            if (a.FindActionMap(requiredMapName, false) != null)
                return a;
        }

        return null;
    }

    static InputActionAsset FirstAssetWithMap(InputActionAsset[] assets, string requiredMapName)
    {
        if (assets == null)
            return null;

        foreach (var asset in assets)
        {
            if (asset == null)
                continue;
            if (asset.FindActionMap(requiredMapName, false) != null)
                return asset;
        }

        return null;
    }
}

