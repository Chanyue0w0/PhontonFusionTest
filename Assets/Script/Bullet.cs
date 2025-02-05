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

    [SerializeField]
    private GameObject blood;
    [SerializeField]
    private GameObject explosion;

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

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.TakeDamage(10);

            // 非網路同步特效
            if (blood != null)
                Instantiate(blood, transform.position, Quaternion.identity);
            Runner.Despawn(Object);
        }

        if (other.CompareTag("Obstacle"))
        {
            // 非網路同步特效
            if(explosion != null)
                Instantiate(explosion, transform.position, Quaternion.identity);
            Runner.Despawn(Object);
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        var player = other.GetComponent<PlayerController>();
    //        player.TakeDamage(10);

    //        // 呼叫 EffectManager 生成血跡特效
    //        EffectManager.Instance.RPC_SpawnEffect("blood", transform.position);
    //        Runner.Despawn(Object);  // 刪除子彈
    //    }

    //    if (other.CompareTag("Obstacle"))
    //    {
    //        // 呼叫 EffectManager 生成爆炸特效
    //        EffectManager.Instance.RPC_SpawnEffect("explosion", transform.position);
    //        Runner.Despawn(Object);  // 刪除子彈
    //    }
    //}


}
