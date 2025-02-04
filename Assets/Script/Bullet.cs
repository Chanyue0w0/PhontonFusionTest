using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; } //Fusion網路用計時器

    [SerializeField]
    private float bulletSpeed = 5f;

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f); //life倒數計時 ， MonoBehaviour自動找到Network Runner
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);//刪除物件
        }
        else
        {
            transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;
        }

    }
}
