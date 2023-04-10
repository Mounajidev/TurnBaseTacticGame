using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;

public class UnitActionSystem : NetworkBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;

    private bool isBusy;

    // Networkk implementation

    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    // network implementation
    public override void Spawned()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UnitActionSystem!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetSelectedUnit(selectedUnit);
    }


    //private void Awake()
    //{
    //    if( Instance != null)
    //    {
    //        Debug.LogError("There is more than one UnitActionSystem!" + transform + "-" + Instance);
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;
    //}

    //private void Start()
    //{
    //    SetSelectedUnit(selectedUnit);
    //}



    public override void FixedUpdateNetwork()
    {
        
        if (isBusy)
        {
            //Debug.Log("IsBusy");
            return;
        }

        if ( !TurnSystem.Instance.IsPlayerTurn())
        {
            
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            
            return;
        }

        if (TryHandleUnitSelection())
        {
            Debug.Log("Try HandleUnitSelection");
            return;

        }
        
        HandleSelectedAction();

        
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Debug.Log(" handle selected action");
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedUnit.TrySpendActionPointsToTakeActions(selectedAction))
            {
                return;
             }
            
             SetBusy();
             selectedAction.RPC_TakeAction(mouseGridPosition, ClearBusy);

             OnActionStarted?.Invoke(this, EventArgs.Empty);

        }
    }

    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);

    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Debug.Log("Button Clicked");
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {

                    if ( unit == selectedUnit)
                    {
                        // Unit is already selected
                        Debug.Log("Unit Already Selected");
                        return false;
                    }

                    if ( unit.IsEnemy())
                    {
                        // Clicked on an Enemy
                        Debug.Log("Enemy Unit Clicked");
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;

        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
