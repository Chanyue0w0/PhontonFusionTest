using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; } //Fusion�����έp�ɾ�

    [SerializeField]
    private float bulletSpeed = 5f;

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f); //life�˼ƭp�� �A MonoBehaviour�۰ʧ��Network Runner
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);//�R������
        }
        else
        {
            transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;
        }

    }
}
