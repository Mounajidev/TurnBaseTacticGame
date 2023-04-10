using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public class TurnSystem : NetworkBehaviour
{

    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;

    //private int turnNumber = 1;
    [Networked] public int turnNumber { get; set; } = 1;

    //private bool isPlayerTurn = true;
    [Networked(OnChanged = nameof(UpdateTurnUI))]
    public NetworkBool isPlayerTurn { get; set; } = true;

    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        Debug.LogError("There is more than one UnitActionSystem!" + transform + "-" + Instance);
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;
    //}
    public override void Spawned()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UnitActionSystem!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    // network OnChange Turn Handler
    // Has to be public static void
    public static void UpdateTurnUI(Changed<TurnSystem> changed)
    {
        changed.Behaviour.OnNetworkTurnChanged();
    }

    private void OnNetworkTurnChanged()
    {
        Debug.Log("Updating UI Turn Visuals");
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NextTurn()
    {
        StartCoroutine(AwaitStateAutAndPassTurn());
        
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            return isPlayerTurn;
        }
        else
        {
            return !isPlayerTurn;
        }
    }

    private IEnumerator AwaitStateAutAndPassTurn()
    {
        Debug.Log("coriutine passing turn .. ");
       
      
            turnNumber++;
            if (Runner.IsSharedModeMasterClient)
            {
                Object.ReleaseStateAuthority();
            yield return new WaitForSeconds(0.2f);
            Object.RequestStateAuthority();
            yield return new WaitForSeconds(0.5f);
            isPlayerTurn = false;
                Debug.Log("server passed the turn" );
            
             }
            else
            {
                Object.ReleaseStateAuthority();
            yield return new WaitForSeconds(0.2f);
            Object.RequestStateAuthority();
            yield return new WaitForSeconds(0.5f);
            isPlayerTurn = true;
                Debug.Log("Is Server turn   :" + Object.HasStateAuthority);
            
             }

            
     }
  
}
