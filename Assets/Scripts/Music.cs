using UnityEngine;

/// <summary>
/// Вешается на слот (PlSlot). Задаёт ноту/мелодию для пластинки в этом слоте.
/// </summary>
public class Music : MonoBehaviour
{
    [Tooltip("Мелодия/нота, которая играет при установке пластинки в этот слот")]
    public AudioClip melodyClip;

    [Range(0f, 1f)]
    public float volume = 0.7f;

    public void PlayAt(Vector3 position)
    {
        if (melodyClip == null)
            return;

        AudioSource.PlayClipAtPoint(melodyClip, position, volume);
    }
}
