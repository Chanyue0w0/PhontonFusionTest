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

    [SerializeField]
    private GameObject blood;
    [SerializeField]
    private GameObject explosion;

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

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.TakeDamage(10);

            // �D�����P�B�S��
            if (blood != null)
                Instantiate(blood, transform.position, Quaternion.identity);
            Runner.Despawn(Object);
        }

        if (other.CompareTag("Obstacle"))
        {
            // �D�����P�B�S��
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

    //        // �I�s EffectManager �ͦ����S��
    //        EffectManager.Instance.RPC_SpawnEffect("blood", transform.position);
    //        Runner.Despawn(Object);  // �R���l�u
    //    }

    //    if (other.CompareTag("Obstacle"))
    //    {
    //        // �I�s EffectManager �ͦ��z���S��
    //        EffectManager.Instance.RPC_SpawnEffect("explosion", transform.position);
    //        Runner.Despawn(Object);  // �R���l�u
    //    }
    //}


}
