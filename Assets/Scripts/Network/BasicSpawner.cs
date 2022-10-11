using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using System;
using System.Linq;

public class BasicSpawner : NetworkBehaviour, INetworkRunnerCallbacks
{
    
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private NetworkPrefabRef _player2Prefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // Network Inputs 
    InputManager _inputManager;

   

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {

        Debug.Log(" Log : " + runner);
        if (runner.IsSharedModeMasterClient)
        {
            Debug.Log("a Player on the team 2 has joined !" + runner);
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 0, 0);
            NetworkObject networkPlayerObject1 = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject1);
            Debug.Log(networkPlayerObject1.HasStateAuthority);
            
        }
        else
        {
            Debug.Log(" client joined, not the server" + runner.IsSharedModeMasterClient);
            
        }
       
     
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) {
        

        //Acces the local unit input handler when it is ready to be access it
        //if (_inputManager != null && _playerPrefab != null)
        //{
            
        //        input.Set(_inputManager.GetNetworkInput());
        //    Debug.Log("OnInput : " + input);
        //}

        //if (_mouseButton0)
        //    data.buttons |= NetworkInputData.MOUSEBUTTON1;
        //Debug.Log("Mouse Button " +  _mouseButton0 + ": " + runner.LocalPlayer);
        //_mouseButton0 = false;

        
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log(" OnInputMissing !" + input);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        Debug.Log(" OnShutDown !" + shutdownReason);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectecToServer" + runner);
    }
    public void OnDisconnectedFromServer(NetworkRunner runner) 
    {
        Debug.Log("OnDisconnectedFromServer" + runner);
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) 
    {
        Debug.Log("OnConnectecRequest" +request + "  ;" + token);
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    {
        Debug.Log("OnConnectecFailed !" + reason);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) 
    {
        Debug.Log("OnUserSimulationMessage" + message);
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated" + sessionList);
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) 
    {
        Debug.Log("OnCustomAuthenticationResponse" + data);
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) 
    {
        Debug.Log("OnHostMigration" + hostMigrationToken);
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) 
    {
        Debug.Log("OnReliableDataReceived" + player + "data:" + data);
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadDone: " + runner);
    }
    public void OnSceneLoadStart(NetworkRunner runner) 
    {
        Debug.Log("OnSceneLoadStart: " + runner);
    }

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            
        });
    }

    private NetworkRunner _runner;

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Shared "))
            {
                StartGame(GameMode.Shared);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Host "))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 80, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}