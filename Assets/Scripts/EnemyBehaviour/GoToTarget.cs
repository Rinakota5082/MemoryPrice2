using UnityEngine;

public class GoToTarget : MonoBehaviour
{
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] Transform player;
    private void Update()
    {
        navMeshAgent.destination = player.position;
    }
}


