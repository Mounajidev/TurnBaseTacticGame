using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class StartCombat : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _turnSystemPrefab;
    [SerializeField] private NetworkPrefabRef _unitActionSystem;
    [SerializeField] private GameObject _playerCanvas;

    [SerializeField] private GameObject _networkCanvasUI;

    [SerializeField] private GameObject _grydSystem;
    [SerializeField] private GameObject _grydVisuals;

    public override void FixedUpdateNetwork()
    {
        if( Input.GetKeyDown(KeyCode.P))
        {
            SpawnCombatSystems();
        }
    }

    public void SpawnCombatSystems()
    {

        StartCoroutine(DelayComponentsSpawner());
        
    }

    public void InstantiateTurnSystem(NetworkRunner runner)
    {
        if (Runner.IsSharedModeMasterClient)
        {
            NetworkObject networkTurnSystem = runner.Spawn(_turnSystemPrefab);
            Debug.Log("turn system loaded ! :" + networkTurnSystem);

        }
        else
        {
            Debug.Log("TurnSystem can only be initialized by the MasterClient");
            return;
        }
    }

    public void InstantiateUnitActionSystem(NetworkRunner runner)
    {
        //GameObject unitActionSystem =
        //           Instantiate(_unitActionSystem, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkObject unitActionSystem = runner.Spawn(_unitActionSystem);
        Debug.Log(unitActionSystem + " Initialized");
    }

    public void InstantiatePlayerCanvas()
    {
        GameObject playerCanvas =
                   Instantiate(_playerCanvas, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log(playerCanvas + " Initialized");
    }

    public void InstantiateGrydSystem()
    {
        GameObject gridSystem =
                   Instantiate(_grydSystem, new Vector3(10, 0, 0), Quaternion.identity);
        Debug.Log(gridSystem + " Initialized");
    }

    public void InstantiateGrydVisuals()
    {
        GameObject grydVisuals =
                   Instantiate(_grydVisuals, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log(grydVisuals + " Initialized");
    }

    public void InstantiateNetworkUICanvas(NetworkRunner runner)
    {
        if (Runner.IsSharedModeMasterClient)
        {
            NetworkObject networkCanvasUI = runner.Spawn(_networkCanvasUI);

        }
        else
        {
            Debug.Log(" Turn Canvas only spawn on server");
        }
    }

    private IEnumerator DelayComponentsSpawner()
    {
        Debug.Log("coriutine instantiating components.. ");
        InstantiateTurnSystem(Runner);
        Debug.Log("turn system loeaded.. ");
        yield return new WaitForSeconds(0.2f);
        
        
        Debug.Log("canvas and grid visual loaded.. ");
        yield return new WaitForSeconds(0.2f);
        InstantiateNetworkUICanvas(Runner);
        Debug.Log("network ui canvas loeaded.. ");
        yield return new WaitForSeconds(0.5f);
        InstantiateUnitActionSystem(Runner);
        InstantiateGrydVisuals();
        InstantiatePlayerCanvas();
        Debug.Log("unit action system loaded.. ");
        
        this.gameObject.SetActive(false);
    }
}
