using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using Cinemachine;


public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterController networkCharacterController = null;

    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private float moveSpeed = 15f;

    [Networked]//此Networked標註表示 Fusion可自動在網路上進行同步
    public NetworkButtons ButtonsPrevious { get; set; }

    [SerializeField]
    private Image hpBar = null;

    [SerializeField]
    private int maxHp = 100; //不需要同步至連線

    [Networked, OnChangedRender(nameof(OnHpChanged))] // 宣告以下變數為Network且若OnHpChanged則執行OnChanged
    public int Hp { get; set; } //需要同步至連線

    [SerializeField]
    private MeshRenderer meshRenderer = null;

    [SerializeField] private GameObject cameraPrefab; // 第三人稱攝影機Prefab
    [SerializeField] private GameObject playerCamera; // 第三人稱玩家攝影機
    [SerializeField] private GameObject cameraFollowTarget;

    public override void Spawned()
    {
        if (Object.HasStateAuthority) //判定是否只在伺服器端運行
            Hp = maxHp;

        // 確保只在本地玩家生成攝影機
        if (Object.HasInputAuthority)
        {
            Debug.Log("生成本地玩家攝影機");

            playerCamera = Instantiate(cameraPrefab);
            playerCamera.tag = "MainCamera"; // 設定為主攝影機

            //// 綁定攝影機跟隨本地玩家
            //CameraFollow cameraFollow = playerCamera.GetComponent<CameraFollow>();
            //if (cameraFollow != null)
            //{
            //    cameraFollow.target = this.transform;
            //}

            // 確認有 CinemachineBrain，必要時添加
            if (playerCamera.GetComponent<CinemachineBrain>() == null)
            {
                playerCamera.AddComponent<CinemachineBrain>();
            }
            CinemachineVirtualCamera virtualCamera = playerCamera.GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                virtualCamera.Follow = cameraFollowTarget.transform;  // 設定跟隨目標
            }


            // 停用預設場景攝影機
            //if (Camera.main != null)
            //{
            //    Camera.main.gameObject.SetActive(false);
            //}
        }
        else
        {
            Debug.Log("其他玩家加入，不生成攝影機");
        }
    }

    private void Respawn()//死亡重生
    {
        networkCharacterController.transform.position = Vector3.up * 2;
        Hp = maxHp;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);// GetPressed比較上一個tik之按下按鈕
            ButtonsPrevious = buttons;

            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveSpeed * moveVector * Runner.DeltaTime);

            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterController.Jump();
            }

            if (pressed.IsSet(InputButtons.FIRE))
            {
                Runner.Spawn(
                    bulletPrefab,
                    transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)),
                    Object.InputAuthority);
            }
        }

        if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)//死亡
        {
            Respawn();
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)//只在伺服器運行
        {
            Hp -= damage;
        }
    }

    //private static void OnHpChanged(Changed<PlayerController> changed)
    //{
    //    changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    //}
    private void OnHpChanged()
    {
        hpBar.fillAmount = (float)Hp / maxHp;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeColor_RPC(Color.red);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeColor_RPC(Color.green);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeColor_RPC(Color.blue);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)] // 使用RPC
    private void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }
}