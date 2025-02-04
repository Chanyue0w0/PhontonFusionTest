using Fusion;
using UnityEngine;

public class EffectManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject bloodEffectPrefab;
    [SerializeField] private NetworkObject explosionEffectPrefab;

    public static EffectManager Instance;

    private void Awake()
    {
        Instance = this;  // ��ҼҦ��A��K��L�}���I�s
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]  // �Ҧ��Ȥ�ݥi�I�s�A���A���t�d�ͦ�
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
