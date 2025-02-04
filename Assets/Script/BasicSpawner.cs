using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef playerPrefab;
    [SerializeField]
    private GameObject cameraPrefab; // 第三人稱攝影機Prefab


    [SerializeField]
    private GameMode gameMode;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();

    private void Start()
    {
        StartGame(gameMode);//GameMode.AutoHostOrClient
    }

    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true; //允許玩家input

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            //Scene = SceneManager.GetActiveScene().buildIndex,
            //Scene = "MyGameScene", // 場景名稱 (需在 Build Settings 裏)
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition = Vector3.up * 2;
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        playerList.Add(player, networkPlayerObject);

        //// 檢查該玩家是否由本地客戶端控制
        //if (networkPlayerObject.HasInputAuthority)
        //{
        //    Debug.Log("本地玩家加入，生成攝影機");

        //    // 生成攝影機
        //    GameObject playerCamera = Instantiate(cameraPrefab);
        //    playerCamera.tag = "MainCamera"; // 設定為主攝影機

        //    // 綁定攝影機到本地玩家物件
        //    CameraFollow cameraFollow = playerCamera.GetComponent<CameraFollow>();
        //    if (cameraFollow != null)
        //    {
        //        cameraFollow.target = networkPlayerObject.transform;
        //    }

        //    // 停用預設場景攝影機
        //    if (Camera.main != null)
        //    {
        //        Camera.main.gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //    Debug.Log("其他玩家加入，不生成攝影機");
        //}

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.movementInput += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.movementInput += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.movementInput += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.movementInput += Vector3.right;

        data.buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space));
        data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0));

        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }
}