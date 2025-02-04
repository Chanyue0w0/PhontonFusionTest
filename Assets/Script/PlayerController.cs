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

    [Networked]//��Networked�е���� Fusion�i�۰ʦb�����W�i��P�B
    public NetworkButtons ButtonsPrevious { get; set; }

    [SerializeField]
    private Image hpBar = null;

    [SerializeField]
    private int maxHp = 100; //���ݭn�P�B�ܳs�u

    [Networked, OnChangedRender(nameof(OnHpChanged))] // �ŧi�H�U�ܼƬ�Network�B�YOnHpChanged�h����OnChanged
    public int Hp { get; set; } //�ݭn�P�B�ܳs�u


    public override void Spawned()
    {
        if (Object.HasStateAuthority) //�P�w�O�_�u�b���A���ݹB��
            Hp = maxHp;
    }

    private void Respawn()//���`����
    {
        networkCharacterController.transform.position = Vector3.up * 2;
        Hp = maxHp;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);// GetPressed����W�@��tik�����U���s
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

        if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)//���`
        {
            Respawn();
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)//�u�b���A���B��
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