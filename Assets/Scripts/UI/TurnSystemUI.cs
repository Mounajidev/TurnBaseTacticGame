using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Fusion;

public class TurnSystemUI : NetworkBehaviour
{

    [SerializeField] private Button endTurnBtn;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    //private void Start()
    //{
        
    //    endTurnBtn.onClick.AddListener(() =>
    //    {
    //        TurnSystem.Instance.NextTurn();
    //    });

    //    TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

    //    UpdateTurnText();
    //    UpdateEnemyTurnVisual();
    //    UpdateEndTurnButtonVisibility();
    //}

    // Network 
    public override void Spawned()
    {
        Debug.Log("turnsystem UI started");
        endTurnBtn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    public void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEnemyTurnVisual()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
            Debug.Log("visual ui on server : " + TurnSystem.Instance.IsPlayerTurn() + "state auth: " + Object.HasStateAuthority );
        }
        else
        {
            enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
            Debug.Log("visual ui on client : " + TurnSystem.Instance.IsPlayerTurn() + "state auth: " + Object.HasStateAuthority);
        }
    }

    private void UpdateEndTurnButtonVisibility()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            endTurnBtn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());

        }
        else
        {
            endTurnBtn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
        }

    }
}
