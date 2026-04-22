using UnityEngine;

public class AgroState : BaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        //Debug.Log("Entering Agro");
        manager.SetSpeed(manager.walkSpeed);
        manager.animator.SetBool("isAgro", true);
        manager.animator.SetBool("isAttacking", false);
    }

    public override void ExitState(EnemyStateManager manager)
    {

    }

    public override void UpdateState(EnemyStateManager manager)
    {
        Debug.Log(manager.DistanceToTarget()+"    2");
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
