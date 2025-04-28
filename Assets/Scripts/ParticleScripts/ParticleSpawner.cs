using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab; // 使用するパーチクルPrefab

    //  四角の中心と大きさを受けてパーチクル生成
    public void PlayParticlesAround(Vector3 center, Vector3 size)
    {
        if (particlePrefab == null)
        {
            Debug.LogWarning("Particle prefab is missing!");
            return;
        }

        Vector3[] corners = new Vector3[]
        {
            center + new Vector3(-size.x/2,  size.y/2, 0),
            center + new Vector3( size.x/2,  size.y/2, 0),
            center + new Vector3(-size.x/2, -size.y/2, 0),
            center + new Vector3( size.x/2, -size.y/2, 0),
        };

        foreach (var corner in corners)
        {
            GameObject p = Instantiate(particlePrefab, corner, Quaternion.identity);
            Destroy(p, 2f); // 2秒後削除
        }
    }
}
