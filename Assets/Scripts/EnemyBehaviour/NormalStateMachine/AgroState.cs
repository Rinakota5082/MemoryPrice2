using UnityEngine;

public class AgroState : BaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        //Debug.Log("Entering Agro");
        manager.SetSpeed(manager.walkSpeed);
        manager.animator.SetBool("IsAgro", true);
        manager.animator.SetBool("IsAttacking", false);
    }

    public override void ExitState(EnemyStateManager manager)
    {

    }

    public override void UpdateState(EnemyStateManager manager)
    {
        if (manager.DistanceToTarget() >= manager.agroDistance)
        {
            manager.SwitchState(manager.idleState);
            return;
        }
        if (manager.DistanceToTarget() < manager.attackDistance)
        {
            manager.SwitchState(manager.attackState);
            return;
        }
    }
}
