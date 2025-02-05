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

    [Networked]//��Networked�е���� Fusion�i�۰ʦb�����W�i��P�B
    public NetworkButtons ButtonsPrevious { get; set; }

    [SerializeField]
    private Image hpBar = null;

    [SerializeField]
    private int maxHp = 100; //���ݭn�P�B�ܳs�u

    [Networked, OnChangedRender(nameof(OnHpChanged))] // �ŧi�H�U�ܼƬ�Network�B�YOnHpChanged�h����OnChanged
    public int Hp { get; set; } //�ݭn�P�B�ܳs�u

    [SerializeField]
    private MeshRenderer meshRenderer = null;

    [SerializeField] private GameObject cameraPrefab; // �ĤT�H����v��Prefab
    [SerializeField] private GameObject playerCamera; // �ĤT�H�٪��a��v��
    [SerializeField] private GameObject cameraFollowTarget;
    private Transform cameraTransform; // ��v�� Transform �Ѧ�

    public override void Spawned()
    {
        if (Object.HasStateAuthority) //�P�w�O�_�u�b���A���ݹB��
            Hp = maxHp;

        // �T�O�u�b���a���a�ͦ���v��
        if (Object.HasInputAuthority)
        {
            Debug.Log("�ͦ����a���a��v��");

            playerCamera = Instantiate(cameraPrefab);
            playerCamera.tag = "MainCamera"; // �]�w���D��v��

            //// �j�w��v�����H���a���a
            //CameraFollow cameraFollow = playerCamera.GetComponent<CameraFollow>();
            //if (cameraFollow != null)
            //{
            //    cameraFollow.target = this.transform;
            //}

            // �T�{�� CinemachineBrain�A���n�ɲK�[
            if (playerCamera.GetComponent<CinemachineBrain>() == null)
            {
                playerCamera.AddComponent<CinemachineBrain>();
            }
            CinemachineVirtualCamera virtualCamera = playerCamera.GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                virtualCamera.Follow = cameraFollowTarget.transform;  // �]�w���H�ؼ�
                cameraTransform = virtualCamera.transform;  // �O�s��v���� Transform �H�Ѳ��ʨϥ�

            }


            // ���ιw�]������v��
            //if (Camera.main != null)
            //{
            //    Camera.main.gameObject.SetActive(false);
            //}
        }
        else
        {
            Debug.Log("��L���a�[�J�A���ͦ���v��");
        }
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

            //if (pressed.IsSet(InputButtons.FIRE))
            //{
            //    Runner.Spawn(
            //        bulletPrefab,
            //        transform.position + transform.TransformDirection(Vector3.forward),
            //        Quaternion.LookRotation(transform.TransformDirection(new Vector3(0,0.1f,1))),
            //        Object.InputAuthority);
            //}
            if (pressed.IsSet(InputButtons.FIRE))
            {
                // �I�s RPC �����A���ͦ��l�u
                FireBullet_RPC(transform.position + transform.TransformDirection(Vector3.forward),
                               transform.rotation);
            }
        }

        if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)//���`
        {
            Respawn();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void FireBullet_RPC(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // �Ѧ��A���ͦ��l�u
        Runner.Spawn(
            bulletPrefab,
            spawnPosition,
            spawnRotation,
            Object.InputAuthority
        );
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)] // �ϥ�RPC
    private void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }
}