using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Runtime safety net to keep locomotion alive when input actions get disabled
/// by controller/UI mediation or simulator toggles.
/// </summary>
public static class LocomotionAutoRecovery
{
    const float RecheckIntervalSeconds = 0.5f;
    static float s_NextCheckTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        s_NextCheckTime = 0f;
        var go = new GameObject("[LocomotionAutoRecovery]");
        Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.DontSave;
        go.AddComponent<LocomotionAutoRecoveryBehaviour>();
    }

    sealed class LocomotionAutoRecoveryBehaviour : MonoBehaviour
    {
        void Update()
        {
            if (Time.unscaledTime < s_NextCheckTime)
                return;

            s_NextCheckTime = Time.unscaledTime + RecheckIntervalSeconds;
            TryRecover();
        }
    }

    static void TryRecover()
    {
        // Movement (avoid newer Unity APIs; use Resources.FindObjectsOfTypeAll)
        var move = FindFirstSceneObject<DynamicMoveProvider>();
        if (move != null && move.gameObject.scene.IsValid())
        {
            if (!move.enabled)
                move.enabled = true;

            EnableActionIfPresent(move.leftHandMoveInput?.inputActionReference);
            EnableActionIfPresent(move.rightHandMoveInput?.inputActionReference);
        }

        // Controller mediation can disable actions globally; ensure Move remains enabled.
        var managers = FindAllSceneObjects<ControllerInputActionManager>();
        if (managers != null && managers.Length != 0)
        {
            foreach (var m in managers)
            {
                if (m != null && m.isActiveAndEnabled)
                {
                    // "Smooth motion" uses Move action.
                    EnableActionIfPresent(GetPrivateActionReference(m, "m_Move"));
                }
            }
        }
    }

    static void EnableActionIfPresent(InputActionReference reference)
    {
        if (reference == null)
            return;

        var action = reference.action;
        if (action == null)
            return;

        // Enabling twice is safe.
        action.Enable();

        // If the whole asset got disabled somewhere, re-enable it too.
        if (action.actionMap != null && !action.actionMap.enabled)
            action.actionMap.Enable();
        if (action.actionMap?.asset != null && !action.actionMap.asset.enabled)
            action.actionMap.asset.Enable();
    }

    static InputActionReference GetPrivateActionReference(ControllerInputActionManager manager, string fieldName)
    {
        var type = typeof(ControllerInputActionManager);
        var f = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return f?.GetValue(manager) as InputActionReference;
    }

    static T FindFirstSceneObject<T>() where T : Object
    {
        var all = Resources.FindObjectsOfTypeAll<T>();
        if (all == null)
            return null;

        foreach (var o in all)
        {
            if (o == null)
                continue;

            if (o is Behaviour b)
            {
                // Ensure it belongs to a loaded scene (not asset/prefab)
                if (!b.gameObject.scene.IsValid())
                    continue;
                return o;
            }

            if (o is Component c)
            {
                if (!c.gameObject.scene.IsValid())
                    continue;
                return o;
            }
        }

        return null;
    }

    static T[] FindAllSceneObjects<T>() where T : Object
    {
        var all = Resources.FindObjectsOfTypeAll<T>();
        if (all == null)
            return new T[0];

        // Filter out assets/prefabs
        var count = 0;
        for (var i = 0; i < all.Length; i++)
        {
            var o = all[i];
            if (o == null)
                continue;

            if (o is Component c && c.gameObject.scene.IsValid())
                count++;
            else if (o is Behaviour b && b.gameObject.scene.IsValid())
                count++;
        }

        if (count == 0)
            return new T[0];

        var result = new T[count];
        var idx = 0;
        for (var i = 0; i < all.Length; i++)
        {
            var o = all[i];
            if (o == null)
                continue;

            if (o is Component c && c.gameObject.scene.IsValid())
                result[idx++] = o;
            else if (o is Behaviour b && b.gameObject.scene.IsValid())
                result[idx++] = o;
        }

        return result;
    }
}

