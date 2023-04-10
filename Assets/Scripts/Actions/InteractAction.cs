using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class InteractAction : BaseAction
{
    private int maxInteractDistance = 1;

    public override void FixedUpdateNetwork()
    {
        if (!isActive)
        {
            return;
        }

    }

    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                LevelGrid.Instance.IsValidGridPosition(testGridPosition);


                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

                if (interactable == null)
                {
                    // No door on this Grid position
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
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);
        Debug.Log("Interact Action");
        RPC_ActionStart(onActionComplete);
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }

  
}
