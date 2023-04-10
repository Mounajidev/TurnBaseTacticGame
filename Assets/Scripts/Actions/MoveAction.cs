using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MoveAction : BaseAction
{

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    [SerializeField] private int maxMoveDistance = 1;

    // Networked variables
    private List<Vector3> positionList;

    //[Networked] [Capacity(20)] [UnitySerializeField]
    //private NetworkLinkedList<Vector3> positionList {  get; }
    //= MakeInitializer(new Vector3[] {  });
    private int currentPositionIndex { get; set; }


    public override void FixedUpdateNetwork()
    {
        if (!isActive)
        {
            return;
        }
       
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Runner.DeltaTime * rotateSpeed);

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Runner.DeltaTime;

        }
        else
        {
            currentPositionIndex++;
            if ( currentPositionIndex >= positionList.Count)
            {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();

            }
        }

    }

    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public override void RPC_TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();
        
        foreach ( GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        RPC_ActionStart(onActionComplete);
        
    }


    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for ( int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for ( int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                LevelGrid.Instance.IsValidGridPosition(testGridPosition);
                

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(unitGridPosition == testGridPosition)
                {
                    //Same Grid Position where the unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position already occupied with another Unit
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultipler = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultipler)
                {
                    // Path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        
        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
