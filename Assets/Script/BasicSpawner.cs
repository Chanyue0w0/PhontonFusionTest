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
    private GameMode gameMode;

    public Transform cameraTransform;  // 指向主攝影機或虛擬攝影機

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
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    //public void OnInput(NetworkRunner runner, NetworkInput input)
    //{
    //    var data = new NetworkInputData();

    //    if (Input.GetKey(KeyCode.W))
    //        data.movementInput += Vector3.forward;

    //    if (Input.GetKey(KeyCode.S))
    //        data.movementInput += Vector3.back;

    //    if (Input.GetKey(KeyCode.A))
    //        data.movementInput += Vector3.left;

    //    if (Input.GetKey(KeyCode.D))
    //        data.movementInput += Vector3.right;

    //    data.buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space));
    //    data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0));

    //    input.Set(data);
    //}
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        // 取得攝影機參考
        Camera playerCamera = Camera.main;  // 確保生成的攝影機有 "MainCamera" 標籤

        if (playerCamera != null)
        {
            // 取得攝影機的前方和右方（忽略Y軸，僅考慮水平移動）
            Vector3 camForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(playerCamera.transform.right, new Vector3(1, 0, 1)).normalized;

            // 根據按鍵輸入計算移動方向
            if (Input.GetKey(KeyCode.W))
                data.movementInput += camForward;

            if (Input.GetKey(KeyCode.S))
                data.movementInput -= camForward;

            if (Input.GetKey(KeyCode.A))
                data.movementInput -= camRight;

            if (Input.GetKey(KeyCode.D))
                data.movementInput += camRight;
        }
        else
        {
            Debug.LogError("未找到攝影機，請確認攝影機已正確標籤為 MainCamera。");
        }

        data.movementInput = data.movementInput.normalized;  // 正規化移動向量

        // 設置其他按鈕輸入
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