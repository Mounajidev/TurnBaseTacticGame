using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class BaseAction : NetworkBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected Unit unit;
    [Networked] protected NetworkBool isActive { get; set; }
    protected Action onActionComplete;

    // network implementation
    public override void Spawned()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    //public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    //networked from above line
    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public abstract void RPC_TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);

       
    }
    
    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    //protected void ActionStart(Action onActionComplete)
    //{
    //    isActive = true;
    //    this.onActionComplete = onActionComplete;

    //    OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    //    Debug.Log("Action started by : " + Runner.LocalPlayer);
    //}

    // RPC converted the method above :
    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    protected void RPC_ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
        Debug.Log("Action started by : " + RpcSources.InputAuthority);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();



        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

            foreach ( GridPosition gridPosition in validActionGridPositionList )
             {
                EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
                enemyAIActionList.Add(enemyAIAction);

              }

            if(enemyAIActionList.Count > 0)
             {

                  enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);

                  return enemyAIActionList[0];
             } else
             {
                  // no possible Enemy AI Actions 
                  return null;
             }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

  
}
