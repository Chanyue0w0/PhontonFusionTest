using Fusion;
using UnityEngine;

public class EffectManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject bloodEffectPrefab;
    [SerializeField] private NetworkObject explosionEffectPrefab;

    public static EffectManager Instance;

    private void Awake()
    {
        Instance = this;  // 單例模式，方便其他腳本呼叫
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]  // 所有客戶端可呼叫，伺服器負責生成
    public void RPC_SpawnEffect(string effectType, Vector3 position)
    {
        NetworkObject effectPrefab = null;

        switch (effectType)
        {
            case "blood":
                effectPrefab = bloodEffectPrefab;
                break;
            case "explosion":
                effectPrefab = explosionEffectPrefab;
                break;
        }

        if (effectPrefab != null)
        {
            Runner.Spawn(effectPrefab, position, Quaternion.identity);
        }
    }
}
