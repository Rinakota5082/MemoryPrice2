using UnityEngine;

public class IdleState : BaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        manager.SetSpeed(0);
        manager.animator.SetBool("isAgro", false);
        manager.animator.SetBool("isAttacking", false);
    }

    public override void ExitState(EnemyStateManager manager)
    {
        Debug.Log("Exited Idle");
    }

    public override void UpdateState(EnemyStateManager manager)
    {
        Debug.Log(manager.DistanceToTarget());
        if (manager.DistanceToTarget() < manager.agroDistance)
        {
            manager.SwitchState(manager.agroState);
            return;
        }
        
    }
}
