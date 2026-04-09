using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(EnemyStateManager manager);

    public abstract void ExitState(EnemyStateManager manager);

    public abstract void UpdateState(EnemyStateManager manager);
}

