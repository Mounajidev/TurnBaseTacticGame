using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GrenadeAction : BaseAction
{
    //[SerializeField] private Transform grenadeProjectilePrefab;

    //Network implementation
    [SerializeField] private NetworkPrefabRef grenadeProjectilePrefab;

    private int maxThrowDistance = 4;

    public override void FixedUpdateNetwork()
    {
        if (!isActive)
        {
            return;
        }

       
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };

    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                LevelGrid.Instance.IsValidGridPosition(testGridPosition);


                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public override void RPC_TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        NetworkObject grenadeProjectileTransform = Runner.Spawn(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition , OnGrenadeBehaviorComplete);

        RPC_ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviorComplete()
    {
        ActionComplete();
    }
}
