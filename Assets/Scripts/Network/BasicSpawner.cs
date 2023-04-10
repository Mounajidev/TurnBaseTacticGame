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
    
    [SerializeField] private NetworkPrefabRef _playerPrefab1;
    [SerializeField] private NetworkPrefabRef _playerPrefab2;
    
    //[SerializeField] private NetworkPrefabRef _networkCanvasUI;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // Network Inputs 
    InputManager _inputManager;

   

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {

        StartCoroutine(AwaitRunnertoLoad(runner, player));
       
     
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
        if (_inputManager != null && _playerPrefab1 != null)
        {

            var data = new NetworkInputData();

            if (Input.GetKey(KeyCode.W))
                Debug.Log(" KeyCode W pressed");
                data.direction += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                data.direction += Vector3.back;

            if (Input.GetKey(KeyCode.A))
                data.direction += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                data.direction += Vector3.right;

            input.Set(data);
        }

        //if (_mouseButton0)
        //    data.buttons |= NetworkInputData.MOUSEBUTTON1;
        //Debug.Log("Mouse Button " + _mouseButton0 + ": " + runner.LocalPlayer);
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
        // spawn Turn System
        //if (Runner.IsSharedModeMasterClient)
        //{
        //NetworkObject networkTurnSystem = runner.Spawn( _turnSystemPrefab );
        //Debug.Log ( "turn system loaded ! :" + networkTurnSystem );

        //}
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
            //if (GUI.Button(new Rect(0, 40, 200, 40), "Host "))
            //{
            //    StartGame(GameMode.Host);
            //}
            //if (GUI.Button(new Rect(0, 80, 200, 40), "Join"))
            //{
            //    StartGame(GameMode.Client);
            //}
        }
    }

    private IEnumerator AwaitRunnertoLoad(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("coriutine  Loading server .. started");
        yield return new WaitForSeconds(1f);
        Debug.Log(" Log : " + runner);
        if (runner.IsSharedModeMasterClient )
        {
            if (_spawnedCharacters.Count > 0)
            {
                Debug.Log("a Player on the team 2 has joined !" + player);
                // Create a unique position for the player
                Vector3 spawnPosition2 = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 2, 0, 10);
                NetworkObject networkPlayerObject2 = runner.Spawn(_playerPrefab2, spawnPosition2, Quaternion.identity, player);
                // Keep track of the player avatars so we can remove it when they disconnect
                _spawnedCharacters.Add(player, networkPlayerObject2);
            }
            else
            {
            Debug.Log("a Player on the team 1 has joined !" + player);
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 6, 0, 0);
            NetworkObject networkPlayerObject1 = runner.Spawn(_playerPrefab1, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject1);

            

            }
            
            
        }
        else
        {
            Debug.Log(" client joined, not the server" + player);

        }
    }
}