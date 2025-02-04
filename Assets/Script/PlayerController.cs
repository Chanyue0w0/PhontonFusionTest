using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

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


    public override void Spawned()
    {
        if (Object.HasStateAuthority) //判定是否只在伺服器端運行
            Hp = maxHp;
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

}