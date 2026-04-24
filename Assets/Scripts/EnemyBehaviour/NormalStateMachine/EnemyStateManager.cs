using UnityEngine;
using UnityEngine.Audio;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] Transform player;
    [SerializeField] public float walkSpeed;
    [SerializeField] public float agroDistance;
    [SerializeField] public float attackDistance;

    [Header("Настройки звука")]
    [SerializeField] private AudioSource audioSource;        
    [SerializeField] private AudioClip agroSoundClip;
    [SerializeField] private AudioClip footstepClip;

    Transform target;
    private bool hasPlayedAgroSound = false;

    BaseState currentState;
    public IdleState idleState = new IdleState();
    public AgroState agroState = new AgroState();
    public AttackState attackState = new AttackState();

    public void SwitchState(BaseState newState)
    {
        if(currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
        if (newState != agroState)
        {
            hasPlayedAgroSound = false;
        }
    }

    private void Start()
    {
        SwitchState(idleState);
    }

    private void Update()
    {
        SetDestination(player);
        navMeshAgent.destination = target.position;
        currentState.UpdateState(this);
    }

    public void SetSpeed(float newSpeed)
    {
        navMeshAgent.speed = newSpeed;
    }

    public void SetDestination(Transform newDestination)
    {
        target = newDestination;
    }

    public float DistanceToTarget()
    {
        return (transform.position - target.transform.position).magnitude;
    }

    public void PlayAgroSound()
    {
        if (!hasPlayedAgroSound && audioSource != null && agroSoundClip != null)
        {
            audioSource.PlayOneShot(agroSoundClip);
            hasPlayedAgroSound = true;
            Debug.Log("[Enemy] Воспроизведение звука агрессии");
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("[Enemy] AudioSource не назначен — звук агрессии не воспроизведён");
        }
        else if (agroSoundClip == null)
        {
            Debug.LogWarning("[Enemy] AudioClip для агрессии не назначен");
        }
    }
    public void PlayFootstepSound()
    {
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }
}
