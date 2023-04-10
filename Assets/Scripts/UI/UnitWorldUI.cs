using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class UnitWorldUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    public override void Spawned()
    {

        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;

        UpdateActionPointsText();
        UpdateHealthBar();

    }
    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateHealthBar()
    {
        Debug.Log("Health" + healthSystem.GetHealthNormalized());
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        Debug.Log("OnDamage ! " + "from server ?  " + Runner.IsSharedModeMasterClient);
        UpdateHealthBar();
    }
}
