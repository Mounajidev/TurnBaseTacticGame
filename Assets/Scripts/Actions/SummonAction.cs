// using System.Collections;
// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Fusion;

// public class SummonAction : BaseAction
// {

//     private string summonType;

//     [SerializeField] private NetworkPrefabRef summonObject;


//     public override void FixedUpdateNetwork()
//     {
//         if (!isActive)
//         {
//             return;
//         }
//         // Create a unique position for the player
//         Vector3 spawnPosition = new Vector3( 2, 0, 0);
//         PlayerRef player = Runner.SetPlayerObject();
//         NetworkObject networkPlayerObject2 = Runner.Spawn(summonObject, spawnPosition, Quaternion.identity, player);

        
//             ActionComplete();
            

//     }

//     //public override void TakeAction(GridPosition gridPosition, Action onActionComplete) 
//     //{

//     //    totalSpinAmount = 0f;
//     //    ActionStart(onActionComplete);
//     //}

//     //Networked from aboce : RPC MEthod
//     //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
//     public override void RPC_TakeAction(GridPosition gridPosition, Action onActionComplete)
//     {
//         RPC_ActionStart(onActionComplete);
//     }

//     public override string GetActionName()
//     {
//         return "Summon Spirit Wolf";
//     }

//     public override List<GridPosition> GetValidActionGridPositionList()
//     {
//         List<GridPosition> validGridPositionList = new List<GridPosition>();
//         GridPosition unitGridPosition = unit.GetGridPosition();

//         return new List<GridPosition>
//         {
//             unitGridPosition
//         };
//     }

//     public override int GetActionPointsCost()
//     {
//         return 4;
//     }

//     public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
//     {
//         return new EnemyAIAction
//         {
//             gridPosition = gridPosition,
//             actionValue = 0,
//         };
//     }
// }
